using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public partial class Database
    {
  

        /**
* [4] signature
* [1] version
* [32] md5_encrypt_md5_options
* [4] groupOffset
* [1] groupCount
* [4] entryOffset
* [2] entryCount
* [4] entryDataOffset
* 
* group
* * [1] nameLength
* * [nameLength] name
* * [1] parentIndex
* 
* entry
* * [1] entryType
* * [1] titleLength
* * [titleLength] title
* * [1] groupIndex
* * [1]? accountLength
* * [accountLength]? account
* * [1] dataCount
* 
* entryData
* [?] dataLength
* [dataLength] data
*/

   

    }
}
