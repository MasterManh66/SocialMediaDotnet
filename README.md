syntax và luồng đi của dữ liệu
Mạng xã hội -> .Net8 + MySQL + EnityFramework

1. Cài Visual Studio -> Dùng ASP.NET and web development
2. dotnet --version | dotnet --list-sdks -> .Net8 ( version 8.0.15)
3. Installed Entity and MySQL
4. Created Database social_media -> config appsetting.json + program.ps connect mySQL social_media
5. dotnet run  | dotnet watch run ( chạy khi thay đổi ) | http://localhost:5082/swagger/index.html
6. Tạo cấu trúc thư mục Controller | Models -> Dto -> Entities | Service
7. Tạo các bảng cơ sở dữ liệu role, user và thêm vào DbContext -> DbSet
8. Chạy lệnh: Add-Migration "commit" |  Update-Database  | Get-Package
	9. DTO ( Data Transfer Object -> request + response -> service -> controller )
	10. Repositories ( truy vấn dữ liệu Entity Framework hoặc SQL ) IUserRepository ( khai bao CRUD ) | UserRepository (thuc thi phuong thuc voi DbContext )
	11. Interface (IUserService -> khai báo các chức năng) & Service -> (UserService -> triển khai logic từ interface)
	12. Controller: Xử lý HTTP request, gọi service trả về response
13. Code Dto ( request + response ) -> Repositories ( IUserRepository + UserRepository ) -> Service ( IUserService + UserService ) -> Controller
14. Config docker redis | docker ps -> docker exec -it ContainerId sh -> redis-cli -> keys * -> ttl result(keys)
15. Hoàn thiện User, Post, Comment, Like, Friend
16. Xuất báo cáo User -> Excel -> dotnet add package ClosedXML -> cấu hình excel -> Dto UserResponse


LINQ
Any : Có ít nhất 1 phần tử thỏa điều kiện không?
All : Tất cả phần tử đều thỏa điều kiện?
Where : Lọc theo điều kiện
Select : Chuyển đổi từng phần tử
FirstOrDefault : Lấy phần tử đầu tiên hoặc null nếu không có
Concat : Nối 2 danh sách lại với nhau
OrderBy : Sắp xếp tăng dần ( ASC mặc định )
OrderByDescending : Sắp xếp giảm dần ( DESC )
ToList : Chuyển từ IEnumerable sang List

HttpStatus
200	: Oki						: Trả về dữ liệu Request thành công (Post, Get, Put)
201 : Created				: Dữ liệu được tạo mới thành công 
204 : No Content		: Thành công nhưng ko có giá trị trả về (Put, Delete)

400 : Bad Request		: Dữ liệu sai cú pháp hoặc không hợp lệ ( thiếu or sai định dạng )
401 : Unauthorized 	: Chưa xác thực ( chưa đăng nhập or token ko hợp lệ )
403 : Forbidden			: Đã xác thực nhưng không có quyền
404 : Not Found			: Không tìm thấy tài nguyên 
409 : Conflict			: Có xung đột trong requets ( trùng tài nguyên )
422 : Unprocessable Entity : Request đúng cú pháp nhưng dữ liệu không hợp lệ ( validate thất bại )

500 : Internal Server Error : Lỗi server không xác định