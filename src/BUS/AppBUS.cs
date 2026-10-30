using System;
using System.Data;
using hoccsharp.DAL;
using hoccsharp.DTO;

namespace hoccsharp.BUS
{
    public class CafeBUS
    {
        private readonly CafeDAL _dal = new CafeDAL();
        public DataTable GetDanhSachBan() => _dal.GetDanhSachBan();
        public DataTable GetThucDon() => _dal.GetThucDon();
        public DataTable GetChiTietHoaDonByBan(int maBan, out int maHoaDon) => _dal.GetChiTietHoaDonByBan(maBan, out maHoaDon);
        public void ThemMonVaoBan(int maBan, int maMon, int soLuong, decimal donGia)
        {
            if (soLuong <= 0) throw new Exception("Số lượng phải > 0!");
            _dal.ThemMonVaoBan(maBan, maMon, soLuong, donGia);
        }
        public bool ThanhToan(int maHoaDon, int maBan, decimal tongTien) => _dal.ThanhToan(maHoaDon, maBan, tongTien);
    }

    public class HotelBUS
    {
        private readonly HotelDAL _dal = new HotelDAL();
        public DataTable GetDanhSachPhong() => _dal.GetDanhSachPhong();
        public KhachHangDTO TimKhachHangByCCCD(string cccd) => _dal.TimKhachHangByCCCD(cccd);
        public void CheckIn(string soPhong, string hoTen, string cccd, string sdt)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(cccd)) throw new Exception("Vui lòng điền đủ Họ tên và CCCD!");
            _dal.CheckIn(soPhong, hoTen, cccd, sdt);
        }
        public decimal CheckOut(string soPhong) => _dal.CheckOut(soPhong);
    }

    public class InventoryBUS
    {
        private readonly InventoryDAL _dal = new InventoryDAL();
        public DataTable GetDanhSachVatTu() => _dal.GetDanhSachVatTu();
        public bool LapPhieuKho(PhieuKhoDTO phieu)
        {
            if (phieu.DanhSachChiTiet.Count == 0) throw new Exception("Chưa chọn mặt hàng nào!");
            return _dal.LapPhieuKho(phieu);
        }
    }
}