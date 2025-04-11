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
15. 