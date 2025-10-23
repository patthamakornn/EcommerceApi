# EcommerceApi

## Overview
โปรเจกต์นี้เป็น Backend API ที่พัฒนาด้วย **ASP.NET 8 Web API** พร้อมใช้งานฐานข้อมูล **PostgreSQL** ผ่าน Entity Framework Core และรันใน Container ด้วย Docker Compose รวมถึงมีระบบความปลอดภัยด้วย JWT สำหรับ Authentication และ Authorization

## Technology Stack
- **Backend**: ASP.NET 8 Web API  
- **Database**: PostgreSQL (เชื่อมต่อผ่าน Entity Framework Core)  
- **Containerization**: Docker + Docker Compose (สำหรับ API และ Database)  
- **Security**: JWT สำหรับ Authentication/Authorization  

---

## Clone & Configuration

1. Clone โปรเจกต์มายังเครื่อง  
    ```bash
    git clone https://github.com/patthamakornn/EcommerceApi.git
    cd EcommerceApi
    ```

2. Build และรัน Container สำหรับ Database และ API  
    ```bash
    docker-compose up --build -d
    ```
    - คำสั่ง `docker-compose up` ใช้สำหรับรันระบบ API และฐานข้อมูล PostgreSQL ภายใน Docker container พร้อมกัน เหมาะสำหรับการทดสอบแบบครบวงจรหรือใช้งานจริง เนื่องจากทุกส่วนถูกรันแยกใน container ทำให้ง่ายต่อการจัดการและแยกสภาพแวดล้อม


3. รัน API ด้วยคำสั่ง  
    ```bash
    dotnet run
    ```
    - คำสั่ง `dotnet run` ใช้สำหรับรัน API บนเครื่องของคุณโดยตรง (ไม่ใช้ container) เหมาะสำหรับการพัฒนาและดีบัก เพราะสามารถแก้ไขโค้ดและทดสอบได้ง่ายโดยไม่ต้องสร้างหรือจัดการ container เพิ่มเติม


4. เปิด Swagger UI เพื่อทดสอบ API ได้ที่  
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
