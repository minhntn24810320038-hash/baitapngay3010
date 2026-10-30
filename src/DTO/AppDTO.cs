using System;
using System.Collections.Generic;

namespace hoccsharp.DTO
{
    // DTO Cà phê
    public class BanAnDTO
    {
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public string TrangThai { get; set; }
    }

    // DTO Khách sạn
    public class KhachHangDTO
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public string SoDienThoai { get; set; }
    }

    // DTO Kho
    public class ChiTietPhieuDTO
    {
        public string MaVatTu { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }

    public class PhieuKhoDTO
    {
        public string LoaiPhieu { get; set; }
        public int? MaNCC { get; set; }
        public List<ChiTietPhieuDTO> DanhSachChiTiet { get; set; } = new List<ChiTietPhieuDTO>();
    }
}