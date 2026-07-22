namespace cloudinvoice_web_ui.DTOs.Clientes
{
    public class OverviewClientesDto
{
        public int TotalClientes { get; set; }
        public int ClientesAtivos { get; set; }
        public int NovosEsteMes { get; set; }
        public double TotalEmDivida { get; set; }
        public  List<InserirClienteDto> ListaClientes { get; set; }
    }
}
