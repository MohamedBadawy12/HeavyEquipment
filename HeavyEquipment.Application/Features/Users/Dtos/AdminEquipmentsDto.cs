using HeavyEquipment.Application.Features.Equipments.Dto;

namespace HeavyEquipment.Application.Features.Users.Dtos
{
    public class AdminEquipmentsDto
    {
        public List<EquipmentSummaryDto> Equipments { get; set; } = new();
        public string? CategoryFilter { get; set; }
        public string? StatusFilter { get; set; }
        public int TotalEquipments { get; set; }
        public int AvailableEquipments { get; set; }
        public int RentedEquipments { get; set; }
    }
}
