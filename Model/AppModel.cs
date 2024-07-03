namespace BackEnd.Model
{
    public class AppModel
    {

    }

    public class ApiResult<T>
    {
        public string Message { get; set; }
        public object Data {  get; set; }
        public bool IsSuccessfull {  get; set; }
    }

    public class ApiResult 
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public bool IsSuccessfull { get; set; }
    }

}
