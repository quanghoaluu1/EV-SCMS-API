# EV-SCMS 🚗⚡
**EV Service Center Maintenance Management System**  
Phần mềm quản lý bảo dưỡng xe điện cho trung tâm dịch vụ, phát triển theo kiến trúc **microservice** với ASP.NET Core.  

---

## 📌 Giới thiệu  
EV-SCMS là hệ thống hỗ trợ các trung tâm dịch vụ quản lý toàn bộ quy trình **bảo dưỡng, sửa chữa, quản lý khách hàng và phụ tùng cho xe điện**.  
Dự án được thiết kế theo kiến trúc **Microservice** để đảm bảo **tính mở rộng, dễ bảo trì và triển khai**.  

---

## 👥 Vai trò trong hệ thống  
- **Customer (Khách hàng):** Đặt lịch, theo dõi xe, quản lý chi phí và thanh toán online.  
- **Staff (Nhân viên):** Tiếp nhận yêu cầu, quản lý dịch vụ và hỗ trợ khách hàng.  
- **Technician (Kỹ thuật viên):** Thực hiện bảo dưỡng, cập nhật tình trạng xe, quản lý checklist.  
- **Admin:** Quản lý nhân sự, phụ tùng, báo cáo, tài chính.  

---

## ⚙️ Chức năng chính  

### 1. Khách hàng (Customer)  
- Nhắc nhở bảo dưỡng định kỳ (theo km/thời gian).  
- Đặt lịch bảo dưỡng/sửa chữa online, chọn trung tâm & dịch vụ.  
- Theo dõi trạng thái xe (chờ – đang bảo dưỡng – hoàn tất).  
- Lưu lịch sử bảo dưỡng, quản lý chi phí.  
- Thanh toán online (e-wallet, banking, …).  

### 2. Trung tâm dịch vụ (Staff/Technician/Admin)  
- Quản lý hồ sơ khách hàng & xe điện.  
- Quản lý lịch hẹn, tiếp nhận & phân công dịch vụ.  
- Theo dõi quy trình bảo dưỡng và tình trạng xe.  
- Quản lý kho phụ tùng, cảnh báo tồn kho tối thiểu.  
- Gợi ý nhu cầu phụ tùng bằng AI.  
- Quản lý nhân sự, ca làm việc, chứng chỉ chuyên môn.  
- Quản lý tài chính: báo giá → hóa đơn → thanh toán.  
- Thống kê dịch vụ phổ biến, báo cáo doanh thu & xu hướng hỏng hóc.  

---

## 🏗️ Kiến trúc Microservice  

Dự án được triển khai theo mô hình **microservice**:  
- Mỗi dịch vụ đảm nhiệm một domain riêng biệt.  
- Các dịch vụ giao tiếp qua API Gateway.  
- Hỗ trợ mở rộng, dễ dàng tích hợp thêm dịch vụ mới.  

### Cấu trúc thư mục (ví dụ)  
```bash
EV-SCMS/
│── ApiGateway/            # Cổng API chung cho client
│── Services/
│   ├── Auth/              # Quản lý đăng nhập/đăng ký, xác thực
│   ├── Customer/          # Dịch vụ quản lý khách hàng & xe
│   ├── Booking/           # Quản lý lịch hẹn & phiếu dịch vụ
│   ├── Maintenance/       # Quản lý quy trình bảo dưỡng & kỹ thuật viên
│   ├── Inventory/         # Quản lý kho phụ tùng
│   ├── ...
│── Shared/                # Các thư viện, tiện ích dùng chung
│── EV-SCMS.sln            # Solution chính
```
### 🛠️ Công nghệ sử dụng

- Backend: ASP.NET Core 8

- Database: PostgreSQL 

- Authentication: JWT / Identity Server

- Communication: REST API (có thể mở rộng gRPC, RabbitMQ, Kafka)
