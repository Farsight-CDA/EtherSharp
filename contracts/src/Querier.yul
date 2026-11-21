object "Querier" {
    code {
        let size := datasize("runtime")
        datacopy(0, dataoffset("runtime"), size)
        return(0, size)
    }
    object "runtime" {
        code {
            // Querier bytecode origin.
            // Raw markers are build-time metadata. Never execute this template directly.

            let inputOffset := 0
            let outputOffset := 0

            for {} lt(inputOffset, calldatasize()) {} {
                let opCode := byte(0, calldataload(inputOffset))
                inputOffset := add(inputOffset, 1)

                // Call
                verbatim_0i_0o(hex"01234567890000")
                if eq(opCode, 0) {
                    let header := calldataload(inputOffset)
                    let length := shr(232, header)
                    calldatacopy(outputOffset, add(inputOffset, 55), length)
                    // CALL uses only the destination's low 160 bits, ignoring the length above them.
                    let success := call(
                        sub(gas(), 50000),
                        shr(72, header),
                        calldataload(add(inputOffset, 23)),
                        outputOffset,
                        length,
                        0,
                        0
                    )
                    mstore(outputOffset, shl(224, returndatasize()))
                    mstore8(outputOffset, success)
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                    inputOffset := add(inputOffset, add(55, length))
                    continue
                }
                verbatim_0i_0o(hex"01234567890000")

                // CallAndMeasureGas
                verbatim_0i_0o(hex"01234567890100")
                if eq(opCode, 1) {
                    let length := shr(232, calldataload(inputOffset))
                    let to := shr(96, calldataload(add(inputOffset, 3)))
                    let value := calldataload(add(inputOffset, 23))

                    inputOffset := add(inputOffset, 55)
                    calldatacopy(outputOffset, inputOffset, length)
                    inputOffset := add(inputOffset, length)

                    {
                        let gasBefore := gas()
                        let success := call(
                            sub(gas(), 50000),
                            to,
                            value,
                            outputOffset,
                            length,
                            0,
                            0
                        )
                        let gasUsed := sub(gasBefore, gas())

                        mstore(
                            outputOffset,
                            or(
                                shl(248, success),
                                or(shl(224, returndatasize()), shl(160, gasUsed))
                            )
                        )
                    }
                    returndatacopy(add(outputOffset, 12), 0, returndatasize())
                    outputOffset := add(outputOffset, add(12, returndatasize()))
                    continue
                }
                verbatim_0i_0o(hex"01234567890100")

                // CreateAndCall
                verbatim_0i_0o(hex"01234567890200")
                if eq(opCode, 2) {
                    let codeLength := shr(240, calldataload(inputOffset))
                    let calldataLength := shr(232, calldataload(add(inputOffset, 2)))
                    let value := calldataload(add(inputOffset, 5))

                    inputOffset := add(inputOffset, 37)
                    let totalLength := add(codeLength, calldataLength)
                    calldatacopy(outputOffset, inputOffset, totalLength)
                    inputOffset := add(inputOffset, totalLength)

                    let success := create(0, outputOffset, codeLength)
                    if success {
                        success := call(
                            sub(gas(), 50000),
                            success,
                            value,
                            add(outputOffset, codeLength),
                            calldataLength,
                            0,
                            0
                        )
                    }
                    mstore(outputOffset, shl(224, returndatasize()))
                    mstore8(outputOffset, success)
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                    continue
                }
                verbatim_0i_0o(hex"01234567890200")

                // Isolate
                verbatim_0i_0o(hex"01234567890300")
                if eq(opCode, 3) {
                    let length := shr(232, calldataload(inputOffset))
                    let value := calldataload(add(inputOffset, 3))

                    inputOffset := add(inputOffset, 35)
                    let selectorLength := shl(2, iszero(eq(extcodesize(address()), codesize())))
                    mstore(outputOffset, shl(224, 0xea41597b))
                    calldatacopy(add(outputOffset, selectorLength), inputOffset, length)
                    inputOffset := add(inputOffset, length)

                    let success := call(
                        sub(gas(), 50000),
                        address(),
                        value,
                        outputOffset,
                        add(length, selectorLength),
                        0,
                        0
                    )
                    mstore(outputOffset, shl(224, returndatasize()))
                    mstore8(outputOffset, success)
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                    continue
                }
                verbatim_0i_0o(hex"01234567890300")

                // PartialRange
                verbatim_0i_0o(hex"01234567890400")
                if eq(opCode, 4) {
                    let header := calldataload(inputOffset)
                    // Includes CALL overhead, coordination, and finalization.
                    let reserve := add(and(shr(200, header), 0xffffffff), 50000)
                    let memoryLimit := and(shr(168, header), 0xffffffff)
                    let inputEnd := add(add(inputOffset, 11), shr(232, header))
                    inputOffset := add(inputOffset, 11)
                    let outputStart := outputOffset
                    outputOffset := add(outputOffset, 3)

                    for {} lt(inputOffset, inputEnd) {} {
                        let length := shr(232, calldataload(inputOffset))

                        // Price input copying before expanding memory. The explicit ceiling
                        // also bounds memory on chains whose memory limit is independent of gas.
                        {
                            let end := add(outputOffset, add(length, 32))
                            if gt(end, memoryLimit) { break }
                            let cost := mul(shr(5, add(length, 31)), 3)
                            if gt(end, msize()) {
                                let words := shr(5, add(end, 31))
                                let previousWords := shr(5, msize())
                                cost := add(cost, sub(
                                    add(mul(words, 3), shr(9, mul(words, words))),
                                    add(mul(previousWords, 3), shr(9, mul(previousWords, previousWords)))
                                ))
                            }
                            if iszero(gt(gas(), add(reserve, cost))) { break }
                        }
                        calldatacopy(outputOffset, add(inputOffset, 55), length)

                        // Reserve gas explicitly rather than relying on an EIP-150 cap.
                        {
                            let available := gas()
                            if iszero(gt(available, reserve)) { break }
                            let success := call(
                                sub(available, reserve),
                                shr(96, calldataload(add(inputOffset, 3))),
                                calldataload(add(inputOffset, 23)),
                                outputOffset,
                                length,
                                0,
                                0
                            )
                            if iszero(success) { break }
                        }

                        // A successful call may still return more data than we can afford.
                        // Do not copy any of it until the complete result fits every budget.
                        {
                            let size := returndatasize()
                            let end := add(outputOffset, add(3, size))
                            if gt(end, memoryLimit) { break }
                            let cost := mul(shr(5, add(size, 31)), 3)
                            if gt(end, msize()) {
                                let words := shr(5, add(end, 31))
                                let previousWords := shr(5, msize())
                                cost := add(cost, sub(
                                    add(mul(words, 3), shr(9, mul(words, words))),
                                    add(mul(previousWords, 3), shr(9, mul(previousWords, previousWords)))
                                ))
                            }
                            if iszero(gt(gas(), add(reserve, cost))) { break }
                            mstore8(outputOffset, shr(16, size))
                            mstore8(add(outputOffset, 1), shr(8, size))
                            mstore8(add(outputOffset, 2), size)
                            returndatacopy(add(outputOffset, 3), 0, size)
                            outputOffset := end
                        }
                        inputOffset := add(inputOffset, add(55, length))
                    }

                    let resultsLength := sub(outputOffset, add(outputStart, 3))
                    mstore8(outputStart, shr(16, resultsLength))
                    mstore8(add(outputStart, 1), shr(8, resultsLength))
                    mstore8(add(outputStart, 2), resultsLength)
                    inputOffset := inputEnd
                    continue
                }
                verbatim_0i_0o(hex"01234567890400")

                // FlashCall, state override variant.
                // The client installs the runtime at the low 20 bytes of initCodeHash.
                verbatim_0i_0o(hex"01234567890501")
                if eq(opCode, 5) {
                    let runtimeLength := shr(240, calldataload(inputOffset))
                    let calldataLength := shr(232, calldataload(add(inputOffset, 2)))
                    let value := calldataload(add(inputOffset, 5))
                    let target := calldataload(add(inputOffset, 37))

                    inputOffset := add(inputOffset, add(69, runtimeLength))
                    calldatacopy(outputOffset, inputOffset, calldataLength)
                    inputOffset := add(inputOffset, calldataLength)

                    // CALL uses only the destination's low 160 bits.
                    let success := call(
                        sub(gas(), 50000),
                        target,
                        value,
                        outputOffset,
                        calldataLength,
                        0,
                        0
                    )
                    mstore(outputOffset, shl(224, returndatasize()))
                    mstore8(outputOffset, success)
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                    continue
                }
                verbatim_0i_0o(hex"01234567890501")

                // FlashCall, deploying variant.
                // Deploys the runtime at create2(address(), 0, initCodeHash) on first use. An Isolate
                // rollback removes the deployment, so the next use deploys it again.
                verbatim_0i_0o(hex"01234567890502")
                if eq(opCode, 5) {
                    let runtimeLength := shr(240, calldataload(inputOffset))
                    let calldataLength := shr(232, calldataload(add(inputOffset, 2)))
                    let value := calldataload(add(inputOffset, 5))

                    mstore(outputOffset, or(shl(160, 0xff), address()))
                    mstore(add(outputOffset, 32), 0)
                    mstore(add(outputOffset, 64), calldataload(add(inputOffset, 37)))
                    let target := keccak256(add(outputOffset, 11), 85)
                    inputOffset := add(inputOffset, 69)

                    let success := 1
                    if iszero(extcodesize(target)) {
                        // PUSH2 length PUSH1 12 RETURNDATASIZE CODECOPY PUSH2 length RETURNDATASIZE RETURN
                        mstore(
                            outputOffset,
                            shl(160, or(0x610000600c3d396100003df3, or(shl(72, runtimeLength), shl(16, runtimeLength))))
                        )
                        calldatacopy(add(outputOffset, 12), inputOffset, runtimeLength)
                        success := create2(0, outputOffset, add(12, runtimeLength), 0)
                    }
                    inputOffset := add(inputOffset, runtimeLength)

                    if success {
                        calldatacopy(outputOffset, inputOffset, calldataLength)
                        success := call(
                            sub(gas(), 50000),
                            target,
                            value,
                            outputOffset,
                            calldataLength,
                            0,
                            0
                        )
                    }
                    inputOffset := add(inputOffset, calldataLength)

                    mstore(outputOffset, shl(224, returndatasize()))
                    mstore8(outputOffset, success)
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                    continue
                }
                verbatim_0i_0o(hex"01234567890502")

                // GetCode
                verbatim_0i_0o(hex"01234567890a00")
                if eq(opCode, 10) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    let codeSize := extcodesize(to)
                    mstore(outputOffset, shl(232, codeSize))
                    extcodecopy(to, add(outputOffset, 3), 0, codeSize)
                    outputOffset := add(outputOffset, add(3, codeSize))
                    continue
                }
                verbatim_0i_0o(hex"01234567890a00")

                // GetCodeHash
                verbatim_0i_0o(hex"01234567890b00")
                if eq(opCode, 11) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, extcodehash(to))
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567890b00")

                // HasCode
                verbatim_0i_0o(hex"01234567890c00")
                if eq(opCode, 12) {
                    inputOffset := add(inputOffset, 20)
                    mstore8(
                        outputOffset,
                        gt(extcodesize(shr(96, calldataload(sub(inputOffset, 20)))), 0)
                    )
                    outputOffset := add(outputOffset, 1)
                    continue
                }
                verbatim_0i_0o(hex"01234567890c00")

                // GetChainId
                verbatim_0i_0o(hex"01234567891400")
                if eq(opCode, 20) {
                    mstore(outputOffset, shl(192, chainid()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"01234567891400")

                // GetBlockNumber
                verbatim_0i_0o(hex"01234567891500")
                if eq(opCode, 21) {
                    mstore(outputOffset, shl(192, number()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"01234567891500")

                // GetBlockTimestamp
                verbatim_0i_0o(hex"01234567891600")
                if eq(opCode, 22) {
                    mstore(outputOffset, shl(192, timestamp()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"01234567891600")

                // GetBlockGasLimit
                verbatim_0i_0o(hex"01234567891700")
                if eq(opCode, 23) {
                    mstore(outputOffset, shl(192, gaslimit()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"01234567891700")

                // GetBlockGasPrice
                verbatim_0i_0o(hex"01234567891800")
                if eq(opCode, 24) {
                    mstore(outputOffset, gasprice())
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567891800")

                // GetBlockBaseFee
                verbatim_0i_0o(hex"01234567891900")
                if eq(opCode, 25) {
                    mstore(outputOffset, basefee())
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567891900")

                // GetBalance
                verbatim_0i_0o(hex"01234567891e00")
                if eq(opCode, 30) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, balance(to))
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567891e00")

                // ReadStorage
                verbatim_0i_0o(hex"01234567891f00")
                if eq(opCode, 31) {
                    let slot := calldataload(inputOffset)
                    inputOffset := add(inputOffset, 32)
                    mstore(outputOffset, sload(slot))
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567891f00")

                // GetNonce
                verbatim_0i_0o(hex"01234567892000")
                if eq(opCode, 32) {
                    mstore(outputOffset, shl(96, address()))
                    mstore(add(outputOffset, 20), shl(96, create(0, 0, 0)))
                    outputOffset := add(outputOffset, 40)
                    continue
                }
                verbatim_0i_0o(hex"01234567892000")

                // GetRemainingGas
                verbatim_0i_0o(hex"01234567892800")
                if eq(opCode, 40) {
                    mstore(outputOffset, gas())
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567892800")

                revert(0, outputOffset)
            }

            return(0, outputOffset)
        }
    }
}
