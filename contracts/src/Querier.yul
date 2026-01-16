object "Querier" {
    code {
        datacopy(0, dataoffset("runtime"), datasize("runtime"))
        return(0, datasize("runtime"))
    }
    object "runtime" {
        code {
            let RETURN_GAS_BUFFER := 50000
            let ABORT_GAS_BUFFER := 55000

            if lt(calldatasize(), 4) { revert(0, 0) }

            let maxReturnSize := shr(224, calldataload(0))
            let inputOffset := 4
            let lastOutputOffset := 0
            let outputOffset := 0

            for {} lt(inputOffset, calldatasize()) {} {
                if gt(outputOffset, maxReturnSize) {
                    return(0, lastOutputOffset)
                }

                lastOutputOffset := outputOffset
                let opCode := byte(0, calldataload(inputOffset))
                inputOffset := add(inputOffset, 1)

                switch opCode
                case 0 {
                    let length := shr(232, calldataload(inputOffset))
                    let to := shr(96, calldataload(add(inputOffset, 3)))
                    let value := calldataload(add(inputOffset, 23))

                    inputOffset := add(inputOffset, 55)
                    calldatacopy(outputOffset, inputOffset, length)
                    inputOffset := add(inputOffset, length)

                    let success := call(
                        sub(gas(), RETURN_GAS_BUFFER),
                        to,
                        value,
                        outputOffset,
                        length,
                        0,
                        0
                    )

                    if and(iszero(success), lt(gas(), ABORT_GAS_BUFFER)) {
                        break
                    }

                    mstore8(outputOffset, success)
                    mstore(add(outputOffset, 1), shl(232, returndatasize()))
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                }
                case 1 {
                    let length := shr(232, calldataload(inputOffset))
                    let to := shr(96, calldataload(add(inputOffset, 3)))
                    let value := calldataload(add(inputOffset, 23))

                    inputOffset := add(inputOffset, 55)
                    calldatacopy(outputOffset, inputOffset, length)
                    inputOffset := add(inputOffset, length)

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

                    if and(iszero(success), lt(gas(), ABORT_GAS_BUFFER)) {
                        break
                    }

                    mstore8(outputOffset, success)
                    mstore(add(outputOffset, 1), shl(224, returndatasize()))
                    mstore(add(outputOffset, 5), shl(192, gasUsed))
                    returndatacopy(add(outputOffset, 13), 0, returndatasize())
                    outputOffset := add(outputOffset, add(13, returndatasize()))
                }
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

                    if and(iszero(success), lt(gas(), ABORT_GAS_BUFFER)) {
                        break
                    }

                    mstore8(outputOffset, success)
                    mstore(add(outputOffset, 1), shl(232, returndatasize()))
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                }
                case 10 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    let codeSize := extcodesize(to)
                    mstore(outputOffset, shl(232, codeSize))
                    extcodecopy(to, add(outputOffset, 3), 0, codeSize)
                    outputOffset := add(outputOffset, add(3, codeSize))
                }
                case 11 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, extcodehash(to))
                    outputOffset := add(outputOffset, 32)
                }
                case 20 {
                    mstore(outputOffset, shl(192, chainid()))
                    outputOffset := add(outputOffset, 8)
                }
                case 21 {
                    mstore(outputOffset, shl(192, number()))
                    outputOffset := add(outputOffset, 8)
                }
                case 22 {
                    mstore(outputOffset, shl(192, timestamp()))
                    outputOffset := add(outputOffset, 8)
                }
                case 23 {
                    mstore(outputOffset, shl(192, gaslimit()))
                    outputOffset := add(outputOffset, 8)
                }
                case 24 {
                    mstore(outputOffset, gasprice())
                    outputOffset := add(outputOffset, 32)
                }
                case 25 {
                    mstore(outputOffset, basefee())
                    outputOffset := add(outputOffset, 32)
                }
                case 30 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)
                    mstore(outputOffset, balance(to))
                    outputOffset := add(outputOffset, 32)
                }
                case 40 {
                    mstore(outputOffset, gas())
                    outputOffset := add(outputOffset, 32)
                }
                default {
                    revert(0, 0)
                }
            }

            if gt(outputOffset, maxReturnSize) {
                return(0, lastOutputOffset)
            }

            return(0, outputOffset)
        }
    }
}