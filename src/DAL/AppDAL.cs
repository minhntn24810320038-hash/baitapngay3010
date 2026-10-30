using System;
using System.Data;
using System.Data.SqlClient;
using hoccsharp.DTO;

namespace hoccsharp.DAL
{
    // DAL Bài 5.3: Cà phê
    public class CafeDAL
    {
        private readonly string _db = "CafePOSDB";

        public DataTable GetDanhSachBan()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM BanAn", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetThucDon()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM MonAn", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetChiTietHoaDonByBan(int maBan, out int maHoaDon)
        {
            maHoaDon = -1;
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlCommand cmdHD = new SqlCommand("SELECT TOP 1 MaHoaDon FROM HoaDon WHERE MaBan = @MaBan AND TrangThai = 0", conn);
                cmdHD.Parameters.AddWithValue("@MaBan", maBan);
                object res = cmdHD.ExecuteScalar();

                if (res != null)
                {
                    maHoaDon = Convert.ToInt32(res);
                    SqlDataAdapter da = new SqlDataAdapter(@"
                        SELECT m.TenMon, c.SoLuong, c.DonGia, (c.SoLuong * c.DonGia) AS ThanhTien
                        FROM ChiTietHoaDon c JOIN MonAn m ON c.MaMon = m.MaMon
                        WHERE c.MaHoaDon = " + maHoaDon, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
                return new DataTable();
            }
        }

        public void ThemMonVaoBan(int maBan, int maMon, int soLuong, decimal donGia)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    int maHD = 0;
                    SqlCommand cmdFind = new SqlCommand("SELECT TOP 1 MaHoaDon FROM HoaDon WHERE MaBan = @MaBan AND TrangThai = 0", conn, trans);
                    cmdFind.Parameters.AddWithValue("@MaBan", maBan);
                    object res = cmdFind.ExecuteScalar();

                    if (res == null)
                    {
                        SqlCommand cmdCreate = new SqlCommand("INSERT INTO HoaDon (MaBan, GioVao, TrangThai) VALUES (@MaBan, GETDATE(), 0); SELECT SCOPE_IDENTITY();", conn, trans);
                        cmdCreate.Parameters.AddWithValue("@MaBan", maBan);
                        maHD = Convert.ToInt32(cmdCreate.ExecuteScalar());

                        SqlCommand cmdUpBan = new SqlCommand("UPDATE BanAn SET TrangThai = N'Có khách' WHERE MaBan = @MaBan", conn, trans);
                        cmdUpBan.Parameters.AddWithValue("@MaBan", maBan);
                        cmdUpBan.ExecuteNonQuery();
                    }
                    else maHD = Convert.ToInt32(res);

                    string sqlCT = @"
                        IF EXISTS (SELECT 1 FROM ChiTietHoaDon WHERE MaHoaDon = @MaHD AND MaMon = @MaMon)
                            UPDATE ChiTietHoaDon SET SoLuong = SoLuong + @SoLuong WHERE MaHoaDon = @MaHD AND MaMon = @MaMon;
                        ELSE
                            INSERT INTO ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia) VALUES (@MaHD, @MaMon, @SoLuong, @DonGia);";
                    SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans);
                    cmdCT.Parameters.AddWithValue("@MaHD", maHD);
                    cmdCT.Parameters.AddWithValue("@MaMon", maMon);
                    cmdCT.Parameters.AddWithValue("@SoLuong", soLuong);
                    cmdCT.Parameters.AddWithValue("@DonGia", donGia);
                    cmdCT.ExecuteNonQuery();

                    trans.Commit();
                }
                catch { trans.Rollback(); throw; }
            }
        }

        public bool ThanhToan(int maHoaDon, int maBan, decimal tongTien)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdHD = new SqlCommand("UPDATE HoaDon SET GioRa = GETDATE(), TongTien = @TongTien, TrangThai = 1 WHERE MaHoaDon = @MaHD", conn, trans);
                    cmdHD.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdHD.Parameters.AddWithValue("@MaHD", maHoaDon);
                    cmdHD.ExecuteNonQuery();

                    SqlCommand cmdBan = new SqlCommand("UPDATE BanAn SET TrangThai = N'Trống' WHERE MaBan = @MaBan", conn, trans);
                    cmdBan.Parameters.AddWithValue("@MaBan", maBan);
                    cmdBan.ExecuteNonQuery();

                    trans.Commit();
                    return true;
                }
                catch { trans.Rollback(); throw; }
            }
        }
    }

    // DAL Bài 5.4: Khách sạn
    public class HotelDAL
    {
        private readonly string _db = "HotelDB";

        public DataTable GetDanhSachPhong()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Phong", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public KhachHangDTO TimKhachHangByCCCD(string cccd)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM KhachHang WHERE CCCD = @CCCD", conn);
                cmd.Parameters.Add("@CCCD", SqlDbType.VarChar, 20).Value = cccd;
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new KhachHangDTO {
                            MaKH = Convert.ToInt32(dr["MaKH"]),
                            HoTen = dr["HoTen"].ToString(),
                            CCCD = dr["CCCD"].ToString(),
                            SoDienThoai = dr["SoDienThoai"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void CheckIn(string soPhong, string hoTen, string cccd, string sdt)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    int maKH = 0;
                    SqlCommand cmdKh = new SqlCommand("SELECT MaKH FROM KhachHang WHERE CCCD = @CCCD", conn, trans);
                    cmdKh.Parameters.AddWithValue("@CCCD", cccd);
                    object res = cmdKh.ExecuteScalar();

                    if (res == null)
                    {
                        SqlCommand cmdAdd = new SqlCommand("INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai) VALUES (@HoTen, @CCCD, @SDT); SELECT SCOPE_IDENTITY();", conn, trans);
                        cmdAdd.Parameters.AddWithValue("@HoTen", hoTen);
                        cmdAdd.Parameters.AddWithValue("@CCCD", cccd);
                        cmdAdd.Parameters.AddWithValue("@SDT", sdt);
                        maKH = Convert.ToInt32(cmdAdd.ExecuteScalar());
                    }
                    else maKH = Convert.ToInt32(res);

                    SqlCommand cmdPhieu = new SqlCommand("INSERT INTO PhieuDatPhong (SoPhong, MaKH, NgayNhanPhong) VALUES (@SoPhong, @MaKH, GETDATE())", conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdPhieu.Parameters.AddWithValue("@MaKH", maKH);
                    cmdPhieu.ExecuteNonQuery();

                    SqlCommand cmdUp = new SqlCommand("UPDATE Phong SET TrangThai = N'Đang ở' WHERE SoPhong = @SoPhong", conn, trans);
                    cmdUp.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdUp.ExecuteNonQuery();

                    trans.Commit();
                }
                catch { trans.Rollback(); throw; }
            }
        }

        public decimal CheckOut(string soPhong)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdPhieu = new SqlCommand(@"
                        SELECT TOP 1 p.MaPhieu, p.NgayNhanPhong, pg.GiaTheoGio 
                        FROM PhieuDatPhong p JOIN Phong pg ON p.SoPhong = pg.SoPhong 
                        WHERE p.SoPhong = @SoPhong AND p.NgayTraPhong IS NULL ORDER BY p.MaPhieu DESC", conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@SoPhong", soPhong);

                    int maPhieu = 0; DateTime ngayNhan = DateTime.Now; decimal giaGio = 0;
                    using (SqlDataReader dr = cmdPhieu.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            maPhieu = Convert.ToInt32(dr["MaPhieu"]);
                            ngayNhan = Convert.ToDateTime(dr["NgayNhanPhong"]);
                            giaGio = Convert.ToDecimal(dr["GiaTheoGio"]);
                        }
                    }

                    if (maPhieu == 0) throw new Exception("Không tìm thấy phòng đang được thuê!");

                    double soGio = Math.Ceiling((DateTime.Now - ngayNhan).TotalHours);
                    if (soGio < 1) soGio = 1;
                    decimal tongTien = (decimal)soGio * giaGio;

                    SqlCommand cmdUpP = new SqlCommand("UPDATE PhieuDatPhong SET NgayTraPhong = GETDATE(), TongTienThanhToan = @TongTien WHERE MaPhieu = @MaPhieu", conn, trans);
                    cmdUpP.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdUpP.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdUpP.ExecuteNonQuery();

                    SqlCommand cmdUpRoom = new SqlCommand("UPDATE Phong SET TrangThai = N'Đang dọn dẹp' WHERE SoPhong = @SoPhong", conn, trans);
                    cmdUpRoom.Parameters.AddWithValue("@SoPhong", soPhong);
                    cmdUpRoom.ExecuteNonQuery();

                    trans.Commit();
                    return tongTien;
                }
                catch { trans.Rollback(); throw; }
            }
        }
    }

    // DAL Bài 5.5: Quản lý Kho
    public class InventoryDAL
    {
        private readonly string _db = "InventoryDB";

        public DataTable GetDanhSachVatTu()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM VatTu", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool LapPhieuKho(PhieuKhoDTO phieu)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection(_db))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdP = new SqlCommand("INSERT INTO PhieuKho (LoaiPhieu, NgayLap, MaNCC, TongTien) VALUES (@LoaiPhieu, GETDATE(), @MaNCC, 0); SELECT SCOPE_IDENTITY();", conn, trans);
                    cmdP.Parameters.AddWithValue("@LoaiPhieu", phieu.LoaiPhieu);
                    cmdP.Parameters.AddWithValue("@MaNCC", (object)phieu.MaNCC ?? DBNull.Value);
                    int maPhieu = Convert.ToInt32(cmdP.ExecuteScalar());

                    decimal tongTien = 0;

                    foreach (var item in phieu.DanhSachChiTiet)
                    {
                        tongTien += item.SoLuong * item.DonGia;

                        SqlCommand cmdCT = new SqlCommand("INSERT INTO ChiTietPhieuKho (MaPhieu, MaVatTu, SoLuong, DonGia) VALUES (@MaPhieu, @MaVatTu, @SoLuong, @DonGia)", conn, trans);
                        cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieu);
                        cmdCT.Parameters.AddWithValue("@MaVatTu", item.MaVatTu);
                        cmdCT.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdCT.Parameters.AddWithValue("@DonGia", item.DonGia);
                        cmdCT.ExecuteNonQuery();

                        if (phieu.LoaiPhieu == "NHAP")
                        {
                            SqlCommand cmdUp = new SqlCommand("UPDATE VatTu SET SoLuongTon = SoLuongTon + @SoLuong WHERE MaVatTu = @MaVatTu", conn, trans);
                            cmdUp.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdUp.Parameters.AddWithValue("@MaVatTu", item.MaVatTu);
                            cmdUp.ExecuteNonQuery();
                        }
                        else if (phieu.LoaiPhieu == "XUAT")
                        {
                            SqlCommand cmdCheck = new SqlCommand("SELECT SoLuongTon FROM VatTu WHERE MaVatTu = @MaVatTu", conn, trans);
                            cmdCheck.Parameters.AddWithValue("@MaVatTu", item.MaVatTu);
                            decimal tonHienTai = Convert.ToDecimal(cmdCheck.ExecuteScalar());

                            if (tonHienTai < item.SoLuong)
                            {
                                trans.Rollback();
                                throw new Exception($"Tồn kho không đủ để xuất! Mã vật tư: {item.MaVatTu} (Hiện có: {tonHienTai})");
                            }

                            SqlCommand cmdUp = new SqlCommand("UPDATE VatTu SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaVatTu = @MaVatTu", conn, trans);
                            cmdUp.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdUp.Parameters.AddWithValue("@MaVatTu", item.MaVatTu);
                            cmdUp.ExecuteNonQuery();
                        }
                    }

                    SqlCommand cmdUpTotal = new SqlCommand("UPDATE PhieuKho SET TongTien = @TongTien WHERE MaPhieu = @MaPhieu", conn, trans);
                    cmdUpTotal.Parameters.AddWithValue("@TongTien", tongTien);
                    cmdUpTotal.Parameters.AddWithValue("@MaPhieu", maPhieu);
                    cmdUpTotal.ExecuteNonQuery();

                    trans.Commit();
                    return true;
                }
                catch { if (trans.Connection != null) trans.Rollback(); throw; }
            }
        }
    }
}