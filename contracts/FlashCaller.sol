// SPDX-License-Identifier: MIT
pragma solidity ^0.8.30;

contract FlashCaller {
    fallback() external {
        assembly {
            let codeLength := shr(240, calldataload(0))
            calldatacopy(0, 2, codeLength)

            let contractAddress := create(0, 0, codeLength)
            let calldataOffset := add(2, codeLength)
            let calldataLength := sub(calldatasize(), calldataOffset)

            calldatacopy(0, calldataOffset, calldataLength)

            let success := call(
                gas(),
                contractAddress,
                callvalue(),
                0,
                calldataLength,
                0,
                0
            )

            mstore8(0, success)
            returndatacopy(1, 0, returndatasize())
            return(0, add(1, returndatasize()))
        }
    }
}
