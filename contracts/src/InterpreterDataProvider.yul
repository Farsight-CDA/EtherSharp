object "InterpreterDataProvider" {
    code {
        let size := datasize("runtime")
        datacopy(0, dataoffset("runtime"), size)
        return(0, size)
    }
    object "runtime" {
        code {
            let selector := shr(224, calldataload(0))

            switch selector
            // bytes4(keccak256("readStorageAndReferencedCode(bytes32)"))
            case 0x772ceff3 {
                let value := sload(calldataload(4))
                mstore(0, value)

                let codeSize := extcodesize(value)
                mstore(32, shl(232, codeSize))
                extcodecopy(value, 35, 0, codeSize)
                return(0, add(35, codeSize))
            }
            default {
                revert(0, 0)
            }
        }
    }
}
