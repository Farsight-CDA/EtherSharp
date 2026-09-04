object "Querier" {
    code {
        let size := datasize("runtime")
        datacopy(0, dataoffset("runtime"), size)
        return(0, size)
    }
    object "runtime" {
        code {
            let RETURN_GAS_BUFFER := 50000

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
                let outputLength := 0
                let opCode := byte(0, calldataload(inputOffset))
                inputOffset := add(inputOffset, 1)

                switch iszero(lt(opCode, 2))
                case 0 {
                    let length := shr(232, calldataload(inputOffset))
                    let to := shr(96, calldataload(add(inputOffset, 3)))
                    let value := calldataload(add(inputOffset, 23))

                    inputOffset := add(inputOffset, 55)
                    calldatacopy(outputOffset, inputOffset, length)
                    inputOffset := add(inputOffset, length)

                    switch opCode
                    case 0 {
                        outputLength := executeCall(to, value, outputOffset, length, outputOffset)
                    }
                    default {
                        let gasBefore := gas()
                        let success := call(
                            sub(gas(), RETURN_GAS_BUFFER),
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
                                or(shl(216, returndatasize()), shl(152, gasUsed))
                            )
                        )
                        returndatacopy(add(outputOffset, 13), 0, returndatasize())
                        outputLength := add(13, returndatasize())
                    }
                }
                default {
                    switch opCode
                case 2 {
                    let codeLength := shr(240, calldataload(inputOffset))
                    let calldataLength := shr(232, calldataload(add(inputOffset, 2)))
                    let value := calldataload(add(inputOffset, 5))

                    inputOffset := add(inputOffset, 37)
                    let totalLength := add(codeLength, calldataLength)

                    calldatacopy(outputOffset, inputOffset, totalLength)
                    inputOffset := add(inputOffset, totalLength)

                    let to := create(0, outputOffset, codeLength)
                    let success := call(
                        sub(gas(), RETURN_GAS_BUFFER),
                        to,
                        value,
                        add(outputOffset, codeLength),
                        calldataLength,
                        0,
                        0
                    )

                    mstore(outputOffset, shl(224, returndatasize()))
                    mstore8(outputOffset, success)
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputLength := add(4, returndatasize())
                }
                case 3 {
                    let length := shr(232, calldataload(inputOffset))
                    let value := calldataload(add(inputOffset, 3))

                    inputOffset := add(inputOffset, 35)
                    mstore(outputOffset, shl(224, 0xea41597b))
                    calldatacopy(add(outputOffset, 4), inputOffset, length)
                    inputOffset := add(inputOffset, length)

                    let to := address()
                    let selectorLength := mul(iszero(eq(extcodesize(to), codesize())), 4)
                    outputLength := executeCall(
                        to,
                        value,
                        add(outputOffset, sub(4, selectorLength)),
                        add(length, selectorLength),
                        outputOffset
                    )
                }
                case 10 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    let codeSize := extcodesize(to)
                    mstore(outputOffset, shl(232, codeSize))
                    extcodecopy(to, add(outputOffset, 3), 0, codeSize)
                    outputLength := add(3, codeSize)
                }
                case 11 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, extcodehash(to))
                    outputLength := 32
                }
                case 12 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore8(outputOffset, gt(extcodesize(to), 0))
                    outputLength := 1
                }
                case 20 {
                    mstore(outputOffset, shl(192, chainid()))
                    outputLength := 8
                }
                case 21 {
                    mstore(outputOffset, shl(192, number()))
                    outputLength := 8
                }
                case 22 {
                    mstore(outputOffset, shl(192, timestamp()))
                    outputLength := 8
                }
                case 23 {
                    mstore(outputOffset, shl(192, gaslimit()))
                    outputLength := 8
                }
                case 24 {
                    mstore(outputOffset, gasprice())
                    outputLength := 32
                }
                case 25 {
                    mstore(outputOffset, basefee())
                    outputLength := 32
                }
                case 30 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, balance(to))
                    outputLength := 32
                }
                case 31 {
                    let slot := calldataload(inputOffset)
                    inputOffset := add(inputOffset, 32)
                    mstore(outputOffset, sload(slot))
                    outputLength := 32
                }
                case 40 {
                    mstore(outputOffset, gas())
                    outputLength := 32
                }
                default {
                    revert(0, outputOffset)
                }
                }

                outputOffset := add(outputOffset, outputLength)
            }

            return(0, outputOffset)
        }
    }
}
