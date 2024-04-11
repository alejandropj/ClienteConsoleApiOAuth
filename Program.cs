// See https://aka.ms/new-console-template for more information
using ClienteConsoleApiOAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

Console.WriteLine("Hello, World!");
Console.WriteLine("Introduzca apellido");
string apellido = Console.ReadLine();
Console.WriteLine("Password");
string pass = Console.ReadLine();

string respuesta = await GetTokenAsync(apellido, pass);
Console.WriteLine(respuesta);

Console.WriteLine("Peticion a Empleado");
string data = await FindEmpleadoAsync(respuesta);
Console.WriteLine(data);

Console.WriteLine("-------------------------");
Console.WriteLine("Fin de programa");


static async Task<string> GetTokenAsync(string user, string pass)
{
    string urlApi = "https://localhost:7195/";
    LoginModel model = new LoginModel
    {
        UserName = user,
        Password = pass
    };
    using (HttpClient client = new HttpClient())
    {
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add
            (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        string jsonModel = JsonConvert.SerializeObject(model);
        StringContent content =
            new StringContent(jsonModel, Encoding.UTF8, "application/json");
        string request = "api/auth/login";
        HttpResponseMessage response = await client.PostAsync(request, content);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            JObject keys = JObject.Parse(data);
            string token = keys.GetValue("response").ToString();
            return token;
        }
        else
        {
            return "Petición incorrecta: " + response.StatusCode;
        }
    }
}

static async Task<string> FindEmpleadoAsync(string token)
{
    string urlApi = "https://localhost:7195/";
    using (HttpClient client = new HttpClient())
    {
        string request = "api/empleados/7839";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add
            (new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
        HttpResponseMessage response = await client.GetAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
        else
        {
            return "Error de algo: " + response.StatusCode;
        }
    }

}