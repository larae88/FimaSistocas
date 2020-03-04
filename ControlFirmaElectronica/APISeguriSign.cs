using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APISeguriSign
{
    public class myListObj
    {
        private string sName;
        private string sID;

        public myListObj()
        {
            sName = "";
            sID = "";
        }

        public myListObj(string theName, string theID)
        {
            sName = theName;
            sID = theID;
        }

        public string Name
        {
            get
            {
                return sName;
            }
            set
            {
                sName = value;
            }
        }


        public string ItemData
        {
            get
            {
                return sID;
            }
            set
            {
                sID = value;
            }
        }

        public override string ToString()
        {
            return sName;
        }
    }
}