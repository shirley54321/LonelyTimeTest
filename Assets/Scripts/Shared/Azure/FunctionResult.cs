namespace Share.Azure 
{
    // Class to represent the result of an Azure function
    public class FunctionResult  
    {
        // Boolean indicating if the function was successful
        public bool success;
    
        // Error message if the function failed
        public string errorMessage;
    }
}