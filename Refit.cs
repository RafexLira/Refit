using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace RefitExample
{
    public interface IProdutoApi
    {
        [Get("/Produto/AllProdutos")]
        Task<List<ProdutoModel>> GetAllProdutos([Header("Authorization")] string authorizationToken);

        [Get("/Produto/ProdutosById/{id}")]
        Task<ProdutoModel> GetProdutoById(int id, [Header("Authorization")] string authorizationToken);
    }

    public class ProdutoService
    {
        private readonly IProdutoApi _produtoApi;

        public ProdutoService(string apiUrl)
        {
            var httpClient = new HttpClient(new AuthenticatedHttpClientHandler());
            _produtoApi = RestService.For<IProdutoApi>(httpClient);
        }

        public async Task<List<ProdutoModel>> GetAllProdutos(string authorizationToken)
        {
            try
            {
                return await _produtoApi.GetAllProdutos(authorizationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter todos os produtos: {ex.Message}");
                return null;
            }
        }

        public async Task<ProdutoModel> GetProdutoById(int id, string authorizationToken)
        {
            try
            {
                return await _produtoApi.GetProdutoById(id, authorizationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter o produto com ID {id}: {ex.Message}");
                return null;
            }
        }
    }

    public class ProdutoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        // Outras propriedades do produto, se houver
    }

    class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            // Insira aqui a lógica para adicionar o token de autorização no cabeçalho da requisição
            request.Headers.Add("Authorization", "Bearer SeuTokenDeAutorizacaoAqui");

            return await base.SendAsync(request, cancellationToken);
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // URL da API Produto
            string apiUrl = "https://suaapi.com/produto";

            // Crie uma instância do ProdutoService passando a URL da API
            var produtoService = new ProdutoService(apiUrl);

            try
            {
                // Token de autorização
                string authorizationToken = "SeuTokenDeAutorizacaoAqui";

                // Obter todos os produtos
                var produtos = await produtoService.GetAllProdutos(authorizationToken);

                if (produtos != null)
                {
                    Console.WriteLine("Todos os produtos:");
                    foreach (var produto in produtos)
                    {
                        Console.WriteLine($"ID: {produto.Id}, Nome: {produto.Nome}, Descrição: {produto.Descricao}");
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum produto encontrado.");
                }

                // Obter um produto por ID
                int produtoId = 1; // ID do produto desejado
                var produtoById = await produtoService.GetProdutoById(produtoId, authorizationToken);

                if (produtoById != null)
                {
                    Console.WriteLine($"\nProduto com ID {produtoId}:");
                    Console.WriteLine($"ID: {produtoById.Id}, Nome: {produtoById.Nome}, Descrição: {produtoById.Descricao}");
                }
                else
                {
                    Console.WriteLine($"Produto com ID {produtoId} não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar a API Produto: {ex.Message}");
            }
        }
    }
}
