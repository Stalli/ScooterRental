using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.TimeProvider
{
    public class DefaultTimeProvider : TimeProvider
    {
        private static DefaultTimeProvider instance;
        
        public static TimeProvider Instance
        {
            get
            {
                if (instance == null)
                    instance = new DefaultTimeProvider();
                return instance;
            }
        }

        public override DateTime Now => DateTime.Now;
    }
}
