# 📌 QUY TẮC COMMIT CODE & QUY TRÌNH LÀM VIỆC VỚI GIT

Tài liệu này mô tả **quy tắc commit code chuẩn**, **quy trình làm việc với Git** và **prompt bắt buộc** để agent / ChatGPT sinh commit message **đúng chuẩn, ổn định và có thể kiểm soát**.

> 📌 **LƯU Ý QUAN TRỌNG**
>
> * Đây là **tài liệu tham chiếu nội bộ**
> * ❌ **KHÔNG commit lại file này** trong các commit code thông thường
> * ✅ Chỉ commit file này khi **thay đổi quy tắc hoặc prompt cho agent**

---

## 1. NGUYÊN TẮC CHUNG

* ✅ **Cho phép commit trực tiếp lên `main`**
* ⚠️ Commit lên `main` **bắt buộc build & test pass**
* ✅ Mỗi commit **chỉ làm MỘT việc rõ ràng**
* ✅ Commit message **bắt buộc có tiêu đề + mô tả chi tiết (body)**
* ❌ Không commit file rác (`node_modules`, `bin`, `obj`, `.env`, ...)
* ✅ Luôn review thay đổi trước khi commit
* ⚠️ Tiêu đề commit **ngắn gọn, không quá dài**

### 1.1. Quy định commit file

#### ✅ BẮT BUỘC commit

* Source code
* File config cần thiết
* `package.json`, `package-lock.json`

#### ❌ KHÔNG commit

* `node_modules/`
* `.env`
* File build sinh tự động (`dist/`, `build/`)

⚠️ **Nếu thêm / xoá dependency → bắt buộc commit cả `package.json` và `package-lock.json`**

---

## 2. QUY ƯỚC COMMIT MESSAGE (CONVENTIONAL COMMITS)

### 2.1. Ngôn ngữ

* ✅ Commit message **bắt buộc viết bằng TIẾNG VIỆT CÓ DẤU**
* ❌ Không dùng tiếng Anh
* ❌ Không dùng từ mơ hồ: `update`, `fix bug`, `chỉnh sửa nhỏ`, `commit cuối`

---

### 2.2. Cấu trúc commit message BẮT BUỘC

```
<type>(<scope>): <tiêu đề ngắn gọn>

<body mô tả chi tiết>
```

#### Quy định chi tiết

* **Tiêu đề**:

  * 1 dòng duy nhất
  * Mô tả đúng hành động chính

* **Body**:

  * Tối thiểu **2 dòng**
  * Mỗi dòng bắt đầu bằng `- `
  * Chỉ mô tả những thay đổi **ĐÃ THỰC SỰ LÀM**

---

### 2.3. Các `type` được phép

| Type     | Ý nghĩa                                    |
| -------- | ------------------------------------------ |
| feat     | Thêm tính năng mới                         |
| fix      | Sửa lỗi                                    |
| refactor | Tối ưu / viết lại code (không đổi hành vi) |
| chore    | Công việc phụ (config, script, build)      |
| docs     | Cập nhật tài liệu                          |
| style    | Format code, không ảnh hưởng logic         |
| test     | Thêm / sửa test                            |

---

### 2.4. Ví dụ commit ĐÚNG CHUẨN

```
feat(exam): hỗ trợ câu hỏi nối trong đề thi

- Thêm loại câu hỏi matching vào cấu trúc đề thi
- Cập nhật logic import đề từ file Excel
- Bổ sung validate dữ liệu cho câu hỏi nối
- Cập nhật mapping đáp án khi chấm điểm
```

---

## 3. QUY TRÌNH LÀM VIỆC CHUẨN VỚI GIT

### Bước 1: Kiểm tra trạng thái local

```bash
git status
```

---

### Bước 2: So sánh code local với Git

```bash
git diff
git diff HEAD
git diff <file-path>
```

---

### Bước 3: Stage file (khuyến nghị chọn lọc)

```bash
git add src/
# hoặc
git add src/controllers/exam.controller.ts
```

❌ Tránh dùng `git add .` khi chưa review kỹ

---

### Bước 4: Kiểm tra trước khi commit

```bash
git status
git diff --staged
```

---

### Bước 5: Commit code

```bash
git commit
```

❌ **KHÔNG dùng commit message mơ hồ**

```
update
fix bug
commit lần cuối
```

---

### Bước 6: Pull trước khi push (BẮT BUỘC)

```bash
git pull origin main
```

---

### Bước 7: Push code

```bash
git push origin main
```

---

## 4. PROMPT BẮT BUỘC DÙNG CHO AGENT / CHATGPT VIẾT COMMIT MESSAGE

> ⚠️ **Agent PHẢI tuân thủ prompt này. Không dùng prompt khác.**

```
Bạn là senior developer và commit message validator.

NHIỆM VỤ DUY NHẤT:
Sinh ra MỘT commit message HỢP LỆ để dùng trực tiếp cho git commit.

========================
RÀNG BUỘC BẮT BUỘC
========================

1. Format CHÍNH XÁC:

<type>(<scope>): <tiêu đề>

<body>

2. Commit message BẮT BUỘC viết bằng TIẾNG VIỆT CÓ DẤU
   - CẤM dùng tiếng Anh
   - CẤM từ mơ hồ

3. Body:
   - Tối thiểu 2 dòng
   - Mỗi dòng bắt đầu bằng "- "
   - Chỉ mô tả những gì ĐÃ LÀM

4. KHÔNG nhắc đến:
   - quy tắc commit
   - prompt
   - tài liệu nội bộ

========================
KIỂM TRA TRƯỚC KHI TRẢ LỜI
========================

Nếu vi phạm BẤT KỲ điều nào → BỎ KẾT QUẢ và VIẾT LẠI.

========================
DANH SÁCH THAY ĐỔI
========================
<PASTE DIFF HOẶC MÔ TẢ THAY ĐỔI VÀO ĐÂY>

CHỈ TRẢ VỀ COMMIT MESSAGE. KHÔNG GIẢI THÍCH.
```

---

## 5. CHECKLIST TRƯỚC KHI COMMIT

* [ ] Code chạy được
* [ ] Không có `console.log` dư thừa
* [ ] Không commit file nhạy cảm
* [ ] Commit message đúng chuẩn
* [ ] Commit có mô tả chi tiết

---

## 6. NGUYÊN TẮC VÀNG

> 🟢 *Một commit tốt = đọc lại sau 6 tháng vẫn hiểu rõ đã làm gì và vì sao làm.*

---

📅 Cập nhật lần cuối: YYYY-MM-DD
👤 Áp dụng cho toàn bộ thành viên dự án
