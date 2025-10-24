// SPDX-License-Identifier: MIT
pragma solidity ^0.8.30;

import {IQuerier} from "./IQuerier.sol";

contract Querier is IQuerier {
    uint constant RETURN_GAS_BUFFER = 200_000;

    function queryCallsAggregated(
        bytes calldata compressedCalls
    ) external returns (bytes memory compressedResults) {
        while (compressedCalls.length > 0) {
            if (gasleft() < RETURN_GAS_BUFFER) {
                break;
            }

            address to = address(bytes20(compressedCalls[0:20]));
            uint32 callDataLength = uint32(bytes4(compressedCalls[20:24]));
            bytes calldata callData = compressedCalls[24:24 + callDataLength];
            compressedCalls = compressedCalls[24 + callDataLength:];

            uint256 gas = gasleft() - RETURN_GAS_BUFFER;
            (bool ok, bytes memory ret) = to.call{gas: gas}(callData);

            compressedResults = abi.encodePacked(
                compressedResults,
                ok,
                uint32(ret.length),
                ret
            );
        }
    }
}
