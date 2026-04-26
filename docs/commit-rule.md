# 📜 Quy tắc Commit (Git Commit Guidelines)

Dự án này tuân thủ chuẩn **Conventional Commits** để đảm bảo lịch sử mã nguồn rõ ràng và dễ tra cứu.

## 🏗️ Cấu trúc một Commit

`<type>(<scope>): <description>`

- **type**: Loại thay đổi (bắt buộc).
- **scope**: Phạm vi thay đổi (tùy chọn, ví dụ: WebApi, Domain, Infrastructure).
- **description**: Mô tả ngắn gọn bằng tiếng Anh hoặc tiếng Việt.

---

## 🏷️ Các loại Type phổ biến

| Type         | Ý nghĩa                                                                   |
| :----------- | :------------------------------------------------------------------------ |
| **feat**     | Thêm một tính năng mới (Feature).                                         |
| **fix**      | Sửa một lỗi (Bug fix).                                                    |
| **docs**     | Thay đổi tài liệu (README, Guide, Checklist).                             |
| **style**    | Thay đổi về định dạng code (khoảng trắng, dấu phẩy...) - không đổi logic. |
| **refactor** | Tái cấu trúc code (không sửa bug cũng không thêm feature).                |
| **perf**     | Cải thiện hiệu năng xử lý.                                                |
| **test**     | Thêm hoặc sửa các Unit Test.                                              |
| **build**    | Thay đổi hệ thống build hoặc dependencies (NuGet, csproj).                |
| **chore**    | Các việc vặt khác (ví cập nhật .gitignore).                               |
