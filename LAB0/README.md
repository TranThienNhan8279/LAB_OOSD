\# Lab 0 - Cài đặt và kiểm tra môi trường



Sinh viên: Trần Thiện Nhân

MSSV: 1250080123



\## Môi trường máy mình



\- Hệ điều hành: Windows \[10/11] 64bit

\- JDK: \[24 LTS / bản mình cài]

\- Maven: 3.9.16

\- Git: 2.53

\- IDE: IntelliJ IDEA 2026.2 (bản Community)



\## Em đã làm gì



Cài lần lượt JDK, Maven, Git, IntelliJ theo hướng dẫn của lab. Sau mỗi bước cài xong đều mở terminal mới lên kiểm tra bằng mấy lệnh: `java -version`, `javac -version`, `mvn -version`, `git --version`.



Sau đó config Git bằng `git config --global user.name` và `user.email`, rồi tạo file `environment.txt` để lưu lại kết quả 4 lệnh kiểm tra trên.



Cuối cùng chạy `git init`, `git add .`, `git commit -m "chore: initialize OOAD lab project"` để khởi tạo repo và commit lần đầu.



\## Kết quả



\- File `environment.txt` đã có đủ kết quả của java, javac, mvn, git

\- `git log --oneline` hiện ra đúng 1 commit đầu tiên

\- \[Nếu làm được] Mở được starter project trong IntelliJ, build bằng `mvn clean package` thành công, chạy được file JAR



\## Vấn đề gặp phải



Lúc tải Maven trên trang chủ Apache thấy có mấy phiên bản (3.9.16, 3.10.0-rc-1, 4.0.0-rc-6) với 2 định dạng file (tar.gz và zip) nên hơi phân vân không biết chọn cái nào. Sau khi đọc kỹ lại thấy lab yêu cầu bản 3.9.16 và vì máy mình là Windows nên phải chọn file zip (không phải tar.gz, cái đó dành cho Linux/macOS), nên mình tải \*\*apache-maven-3.9.16-bin.zip\*\*.



Tương tự lúc cài IntelliJ cũng không biết nên tick chọn gì ở màn hình cài đặt (Desktop Shortcut, Update PATH, Create Associations...). Cuối cùng mình chỉ tick tạo shortcut ngoài desktop và "Open Folder as Project" cho tiện, còn phần Update PATH thì không tick vì IntelliJ là app có giao diện, không cần gọi qua terminal như JDK/Maven/Git.



Ngoài ra sau khi cài JDK xong chạy `java -version` bị báo lỗi "not recognized" — do terminal cũ đang mở vẫn giữ PATH cũ, phải đóng lại và mở terminal mới thì mới nhận được lệnh.

