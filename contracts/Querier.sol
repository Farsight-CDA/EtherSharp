// SPDX-License-Identifier: MIT
pragma solidity ^0.8.30;

contract Querier {
    uint constant RETURN_GAS_BUFFER = 20_000;
    uint constant ABORT_GAS_BUFFER = 30_000;

    fallback() external {
        assembly {
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
                //call(uint24 length,address to,uint256 value,bytes calldata)
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

                    if and(not(success), lt(gas(), ABORT_GAS_BUFFER)) {
                        break
                    }

                    mstore8(outputOffset, success)
                    mstore(add(outputOffset, 1), shl(232, returndatasize()))
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                }
                //callAndMeasureGas(uint24 length,address to,uint256 value,bytes calldata)
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

                    if and(not(success), lt(gas(), ABORT_GAS_BUFFER)) {
                        break
                    }

                    mstore8(outputOffset, success)
                    mstore(add(outputOffset, 1), shl(224, returndatasize()))
                    mstore(add(outputOffset, 5), shl(192, gasUsed))
                    returndatacopy(add(outputOffset, 13), 0, returndatasize())
                    outputOffset := add(outputOffset, add(13, returndatasize()))
                }
                //flash_call(uint16 codeLength, uint24 calldataLength, uint256 callValue, bytes code, bytes calldata)
                case 2 {
                    let codeLength := shr(240, calldataload(inputOffset))
                    let calldataLength := shr(
                        232,
                        calldataload(add(inputOffset, 2))
                    )
                    let value := calldataload(add(inputOffset, 5))

                    inputOffset := add(inputOffset, 37)
                    let totalLength := add(codeLength, calldataLength)

                    calldatacopy(outputOffset, inputOffset, totalLength)

                    inputOffset := add(inputOffset, totalLength)

                    //ToDo: Move into seperate method and gas limit the creation
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

                    if and(not(success), lt(gas(), ABORT_GAS_BUFFER)) {
                        break
                    }

                    mstore8(outputOffset, success)
                    mstore(add(outputOffset, 1), shl(232, returndatasize()))
                    returndatacopy(add(outputOffset, 4), 0, returndatasize())
                    outputOffset := add(outputOffset, add(4, returndatasize()))
                }
                //getCode(address to)
                case 10 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)

                    let codeSize := extcodesize(to)
                    mstore(outputOffset, shl(232, codeSize))
                    extcodecopy(to, add(outputOffset, 3), 0, codeSize)

                    outputOffset := add(outputOffset, add(3, codeSize))
                }
                //getCodeHash(address to)
                case 11 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)

                    mstore(outputOffset, extcodehash(to))
                    outputOffset := add(outputOffset, 32)
                }
                //getChainId
                case 20 {
                    mstore(outputOffset, shl(192, chainid()))
                    outputOffset := add(outputOffset, 8)
                }
                //getBlockNumber()
                case 21 {
                    mstore(outputOffset, shl(192, number()))
                    outputOffset := add(outputOffset, 8)
                }
                //getBlockTimestamp()
                case 22 {
                    mstore(outputOffset, shl(192, timestamp()))
                    outputOffset := add(outputOffset, 8)
                }
                //getBlockGasLimit()
                case 23 {
                    mstore(outputOffset, shl(192, gaslimit()))
                    outputOffset := add(outputOffset, 8)
                }
                //getBlockGasPrice()
                case 24 {
                    mstore(outputOffset, gasprice())
                    outputOffset := add(outputOffset, 32)
                }
                //getBaseFee()
                case 25 {
                    mstore(outputOffset, basefee())
                    outputOffset := add(outputOffset, 32)
                }
                //getBalance(address to)
                case 30 {
                    let to := shr(96, calldataload(inputOffset))
                    inputOffset := add(inputOffset, 20)

                    mstore(outputOffset, balance(to))
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
