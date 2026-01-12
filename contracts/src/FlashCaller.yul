object "FlashCaller" {
    code {
        let size := datasize("runtime")
        datacopy(0, dataoffset("runtime"), size)
        mstore(size, number())
        return(0, add(size, 32))
    }
    object "runtime" {
        code {
            if eq(shr(224, calldataload(0)), 0x217CD3E1) {
                codecopy(0, sub(codesize(), 32), 32)
                return(0, 32)
            }

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