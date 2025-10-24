// SPDX-License-Identifier: MIT
pragma solidity ^0.8.30;

interface IQuerier {
    function queryCallsAggregated(
        bytes calldata compressedCalls
    ) external returns (bytes memory);
}
