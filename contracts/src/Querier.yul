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

            function executeCall(to, value, inputOffset, inputLength, outputOffset) -> outputLength {
                let success := call(
                    sub(gas(), 50000),
                    to,
                    value,
                    inputOffset,
                    inputLength,
                    0,
                    0
                )

                mstore(outputOffset, shl(224, returndatasize()))
                mstore8(outputOffset, success)
                returndatacopy(add(outputOffset, 4), 0, returndatasize())
                outputLength := add(4, returndatasize())
            }

            let inputOffset := 0
            let outputOffset := 0

            for {} lt(inputOffset, calldatasize()) {} {
                let opCode := byte(0, calldataload(inputOffset))
                inputOffset := add(inputOffset, 1)

                // Call.
                verbatim_0i_0o(hex"012345678900")
                if eq(opCode, 0) {
                    let header := calldataload(inputOffset)
                    let length := shr(232, header)
                    let to := shr(96, calldataload(add(inputOffset, 3)))
                    let value := calldataload(add(inputOffset, 23))

                    inputOffset := add(inputOffset, 55)
                    calldatacopy(outputOffset, inputOffset, length)
                    inputOffset := add(inputOffset, length)
                    outputOffset := add(outputOffset, executeCall(to, value, outputOffset, length, outputOffset))
                    continue
                }
                verbatim_0i_0o(hex"012345678900")

                // CallAndMeasureGas.
                verbatim_0i_0o(hex"012345678901")
                if eq(opCode, 1) {
                    let header := calldataload(inputOffset)
                    let length := shr(232, header)
                    let to := shr(96, calldataload(add(inputOffset, 3)))
                    let value := calldataload(add(inputOffset, 23))

                    inputOffset := add(inputOffset, 55)
                    calldatacopy(outputOffset, inputOffset, length)
                    inputOffset := add(inputOffset, length)

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
                    returndatacopy(add(outputOffset, 12), 0, returndatasize())
                    outputOffset := add(outputOffset, add(12, returndatasize()))
                    continue
                }
                verbatim_0i_0o(hex"012345678901")

                // FlashCall.
                verbatim_0i_0o(hex"012345678902")
                if eq(opCode, 2) {
                    let codeLength := shr(240, calldataload(inputOffset))
                    let calldataLength := shr(232, calldataload(add(inputOffset, 2)))
                    let value := calldataload(add(inputOffset, 5))

                    inputOffset := add(inputOffset, 37)
                    let totalLength := add(codeLength, calldataLength)
                    calldatacopy(outputOffset, inputOffset, totalLength)
                    inputOffset := add(inputOffset, totalLength)

                    let to := create(0, outputOffset, codeLength)
                    outputOffset := add(outputOffset, executeCall(
                        to,
                        value,
                        add(outputOffset, codeLength),
                        calldataLength,
                        outputOffset
                    ))
                    continue
                }
                verbatim_0i_0o(hex"012345678902")

                // Isolate.
                verbatim_0i_0o(hex"012345678903")
                if eq(opCode, 3) {
                    let length := shr(232, calldataload(inputOffset))
                    let value := calldataload(add(inputOffset, 3))

                    inputOffset := add(inputOffset, 35)
                    mstore(outputOffset, shl(224, 0xea41597b))
                    calldatacopy(add(outputOffset, 4), inputOffset, length)
                    inputOffset := add(inputOffset, length)

                    let to := address()
                    let selectorLength := mul(iszero(eq(extcodesize(to), codesize())), 4)
                    outputOffset := add(outputOffset, executeCall(
                        to,
                        value,
                        add(outputOffset, sub(4, selectorLength)),
                        add(length, selectorLength),
                        outputOffset
                    ))
                    continue
                }
                verbatim_0i_0o(hex"012345678903")

                // GetCode.
                verbatim_0i_0o(hex"01234567890a")
                if eq(opCode, 10) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    let codeSize := extcodesize(to)
                    mstore(outputOffset, shl(232, codeSize))
                    extcodecopy(to, add(outputOffset, 3), 0, codeSize)
                    outputOffset := add(outputOffset, add(3, codeSize))
                    continue
                }
                verbatim_0i_0o(hex"01234567890a")

                // GetCodeHash.
                verbatim_0i_0o(hex"01234567890b")
                if eq(opCode, 11) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, extcodehash(to))
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567890b")

                // HasCode.
                verbatim_0i_0o(hex"01234567890c")
                if eq(opCode, 12) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore8(outputOffset, gt(extcodesize(to), 0))
                    outputOffset := add(outputOffset, 1)
                    continue
                }
                verbatim_0i_0o(hex"01234567890c")

                // GetChainId.
                verbatim_0i_0o(hex"012345678914")
                if eq(opCode, 20) {
                    mstore(outputOffset, shl(192, chainid()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"012345678914")

                // GetBlockNumber.
                verbatim_0i_0o(hex"012345678915")
                if eq(opCode, 21) {
                    mstore(outputOffset, shl(192, number()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"012345678915")

                // GetBlockTimestamp.
                verbatim_0i_0o(hex"012345678916")
                if eq(opCode, 22) {
                    mstore(outputOffset, shl(192, timestamp()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"012345678916")

                // GetBlockGasLimit.
                verbatim_0i_0o(hex"012345678917")
                if eq(opCode, 23) {
                    mstore(outputOffset, shl(192, gaslimit()))
                    outputOffset := add(outputOffset, 8)
                    continue
                }
                verbatim_0i_0o(hex"012345678917")

                // GetBlockGasPrice.
                verbatim_0i_0o(hex"012345678918")
                if eq(opCode, 24) {
                    mstore(outputOffset, gasprice())
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"012345678918")

                // GetBlockBaseFee.
                verbatim_0i_0o(hex"012345678919")
                if eq(opCode, 25) {
                    mstore(outputOffset, basefee())
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"012345678919")

                // GetBalance.
                verbatim_0i_0o(hex"01234567891e")
                if eq(opCode, 30) {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, balance(to))
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567891e")

                // ReadStorage.
                verbatim_0i_0o(hex"01234567891f")
                if eq(opCode, 31) {
                    let slot := calldataload(inputOffset)
                    inputOffset := add(inputOffset, 32)
                    mstore(outputOffset, sload(slot))
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"01234567891f")

                // GetRemainingGas.
                verbatim_0i_0o(hex"012345678928")
                if eq(opCode, 40) {
                    mstore(outputOffset, gas())
                    outputOffset := add(outputOffset, 32)
                    continue
                }
                verbatim_0i_0o(hex"012345678928")

                revert(0, outputOffset)
            }

            return(0, outputOffset)
        }
    }
}
