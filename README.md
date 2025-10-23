# EcommerceApi

โปรเจกต์ API สำหรับระบบ E-Commerce พร้อมฐานข้อมูล Postgres และ Swagger UI

---

## ขั้นตอนการติดตั้งและใช้งาน

1. Clone โปรเจกต์มายังเครื่อง  
    ```bash
    git clone https://github.com/patthamakornn/EcommerceApi.git
    cd EcommerceApi
    ```

2. Build และรัน Container ด้วย Docker Compose
    ```bash
    docker-compose ps
    ```

3. Run Database Migration 
    ```bash
    dotnet ef database update --project ECommerceApi.Infrastructure --startup-project ECommerceApi.API
    ```
    - สร้าง Migration ตัวแรก (ถ้ายังไม่มี)
    ```bash
    dotnet ef migrations add InitialCreate --project ECommerceApi.Infrastructure --startup-project ECommerceApi.API
    ```

4. Run API
     ```bash
    cd EcommerceApi.API
    dotnet run
    ```

5. เปิด Swagger UI เพื่อทดสอบ API ได้ที่  
    [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

---

## การหยุดและจัดการ Container

- หยุดและลบ Container ทั้งหมด (ข้อมูลใน Volume ของ Database จะยังคงอยู่)  
    ```bash
    docker-compose down
    ```

- หากต้องการลบข้อมูลทั้งหมดใน Database รวมถึง Volume ให้ใช้คำสั่ง  
    ```bash
    docker-compose down -v
    ```

---

## หมายเหตุ

- ตรวจสอบให้แน่ใจว่าคุณได้ติดตั้ง Docker และ .NET 8 SDK แล้วก่อนใช้งาน  
- Docker Compose จะช่วยจัดการ API และ Database รันพร้อมกันอย่างสะดวก  
- คำสั่ง `dotnet run` ใช้สำหรับรัน API แยกในเครื่อง (เหมาะสำหรับการพัฒนาและดีบัก)  

---

หากมีคำถามหรือปัญหาใด ๆ สามารถเปิด Issue ใน repository ได้เลยครับ
