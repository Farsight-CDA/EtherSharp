#!/usr/bin/env node
// Compile the marked Querier and print its bindings as JSON.
import assert from "node:assert/strict";
import { execFileSync } from "node:child_process";
import { createHash } from "node:crypto";
import { readFileSync } from "node:fs";

const SOLC_VERSION = "0.8.30";
const SOURCE = new URL("src/Querier.yul", import.meta.url);
const ORIGIN_ANCHOR = "// Querier bytecode origin.";
const MARKER_PREFIX = Buffer.from("0123456789", "hex");
const MARKER_LENGTH = MARKER_PREFIX.length + 1;
const ORIGINS = [4096, 8192];
const OPTIMIZER = {
    enabled: true,
    runs: 1,
    details: { yul: false, inliner: false, deduplicate: false },
};

function compileRuntime(solc, source, origin) {
    const padding = `verbatim_0i_0o(hex"${"5b".repeat(origin)}")`;
    source = source.replace(ORIGIN_ANCHOR, `${ORIGIN_ANCHOR}\n            ${padding}`);
    const request = {
        language: "Yul",
        sources: { "Querier.yul": { content: source } },
        settings: {
            evmVersion: "london",
            optimizer: OPTIMIZER,
            outputSelection: { "*": { "*": ["evm.deployedBytecode.object"] } },
        },
    };
    const result = JSON.parse(execFileSync(solc, ["--standard-json"], {
        input: JSON.stringify(request),
        encoding: "utf8",
    }));
    const errors = (result.errors ?? []).filter(entry => entry.severity === "error");
    assert(errors.length === 0, errors.map(entry => entry.formattedMessage).join("\n"));
    const runtime = Buffer.from(result.contracts["Querier.yul"].Querier.evm.deployedBytecode.object, "hex");
    assert(
        runtime.length >= origin && runtime.subarray(0, origin).every(value => value === 0x5b),
        "The padding must precede all runtime code, including helpers.",
    );
    return runtime.subarray(origin);
}

function readTemplate(code) {
    const regions = [];
    const instructions = [];
    const segments = [];
    let openRegion;
    let offset = 0;
    for (let rawOffset = 0; rawOffset < code.length;) {
        // Markers are opaque metadata; PUSH operands are skipped below.
        if (code.subarray(rawOffset, rawOffset + MARKER_PREFIX.length).equals(MARKER_PREFIX)) {
            assert(rawOffset + MARKER_LENGTH <= code.length, "Truncated opcode marker.");
            const opcode = code[rawOffset + MARKER_PREFIX.length];
            if (openRegion === undefined) {
                assert(!regions.some(region => region.opcode === opcode), `Duplicate markers for opcode ${opcode}.`);
                openRegion = { opcode, offset };
            } else {
                assert(openRegion.opcode === opcode, "Nested or mismatched opcode markers.");
                regions.push({ ...openRegion, length: offset - openRegion.offset });
                openRegion = undefined;
            }
            rawOffset += MARKER_LENGTH;
            continue;
        }
        const opcode = code[rawOffset];
        const length = 1 + (opcode >= 0x60 && opcode <= 0x7f ? opcode - 0x5f : 0);
        assert(rawOffset + length <= code.length, `Truncated PUSH at offset ${rawOffset}.`);
        const bytes = code.subarray(rawOffset, rawOffset + length);
        instructions.push({ offset, rawOffset, bytes });
        segments.push(bytes);
        rawOffset += length;
        offset += length;
    }
    assert(regions.length > 0 && openRegion === undefined, "Missing opcode markers or unclosed region.");
    return { runtime: Buffer.concat(segments), regions, instructions };
}

function inspect(solc) {
    const version = execFileSync(solc, ["--version"], { encoding: "utf8" });
    assert(version.includes(`Version: ${SOLC_VERSION}+commit.`), `Expected native Solc ${SOLC_VERSION}; received ${version.trim()}.`);
    const source = readFileSync(SOURCE, "utf8");
    assert(source.split(ORIGIN_ANCHOR).length === 2, "Expected one bytecode-origin anchor.");
    const [left, right] = ORIGINS.map(origin => readTemplate(compileRuntime(solc, source, origin)));
    assert.deepEqual(left.regions, right.regions, "Changing the origin changed the opcode regions.");
    assert(left.instructions.length === right.instructions.length, "Changing the origin changed the instruction count.");
    const destinations = new Map(left.instructions.filter(item => item.bytes[0] === 0x5b).map(item => [item.rawOffset, item.offset]));
    const owner = offset => left.regions.find(region => region.offset <= offset && offset < region.offset + region.length)?.opcode;
    const references = [];
    for (let i = 0; i < left.instructions.length; i++) {
        const a = left.instructions[i];
        const b = right.instructions[i];
        assert(a.rawOffset === b.rawOffset && a.bytes[0] === b.bytes[0] && a.bytes.length === b.bytes.length, `Instruction layout changed at ${a.rawOffset}.`);
        if (a.bytes.equals(b.bytes)) {
            continue;
        }
        assert(a.bytes[0] === 0x61, `Expected a PUSH2 relocation at ${a.rawOffset}.`);
        const rawTarget = a.bytes.readUInt16BE(1) - ORIGINS[0];
        assert(rawTarget === b.bytes.readUInt16BE(1) - ORIGINS[1], `Unexpected origin-dependent immediate at ${a.rawOffset}.`);
        const target = destinations.get(rawTarget);
        assert(target !== undefined, `Relocation at ${a.rawOffset} does not target a JUMPDEST.`);
        assert(owner(target) === undefined || owner(target) === owner(a.offset), "A relocation crosses into another removable region.");
        left.runtime.writeUInt16BE(target, a.offset + 1);
        references.push({ offset: a.offset, target });
    }
    assert(left.runtime.length <= 24576, "The complete runtime exceeds the EVM code-size limit.");
    return { source, runtime: left.runtime, regions: left.regions, references };
}

try {
    const args = process.argv.slice(2);
    assert(args.length === 0 || (args.length === 2 && args[0] === "--solc"), "Usage: node contracts/inspect_querier.mjs [--solc /path/to/solc-0.8.30]");
    const { source, runtime, regions, references } = inspect(args[1] ?? process.env.SOLC ?? "solc");
    console.log(JSON.stringify({
        sourceSha256: createHash("sha256").update(source).digest("hex"),
        runtime: runtime.toString("hex"),
        regions,
        references,
    }, null, 2));
} catch (error) {
    console.error(`Querier inspection failed: ${error.message}`);
    process.exitCode = 1;
}
