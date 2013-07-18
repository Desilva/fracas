using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace StarEnergi.Utilities
{

    /*
     * error handling
     * return error message and status
     *      0 => success
     *      1 => fail
     *      2 => data exist
     * 
     */
    public class ErrorHandling
    {
        public Dictionary<string,string> errors { get; set; }
        //
        // 0 -> succes
        // 1 -> fail 
        // 
        public string status;

        public ErrorHandling() {
            errors = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Succes(string message) {
            errors["status"] = "0";
            errors["message"] = message;
            return errors;
        }

        public Dictionary<string, string> Fail(ModelStateDictionary error) {

            errors["status"] = "1";
            foreach (string x in error.Keys)
            {
                if (error[x].Errors.Count != 0)
                {
                    errors[x] = error[x].Errors.First().ErrorMessage;
                }
            }
            return errors;
        }

        public Dictionary<string, string> Fail(IEnumerable<DbEntityValidationResult> error)
        {
            errors["status"] = "1";
            foreach(DbValidationError x in error.First().ValidationErrors){
                errors[x.PropertyName] = x.ErrorMessage;
            }
            return errors;
        }

        //sudah ada di database
        public Dictionary<string, string> Fail()
        {
            errors["status"] = "2";
            return errors;
        }
    }
}