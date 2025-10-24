# EcommerceApi

โปรเจกต์ API สำหรับระบบ E-Commerce พร้อมฐานข้อมูล Postgres และ Swagger UI

---

## ขั้นตอนการติดตั้งและใช้งาน

1. Clone โปรเจกต์มายังเครื่อง  
    ```bash
    git clone https://github.com/patthamakornn/EcommerceApi.git
    cd EcommerceApi
    ```

2. Build และ Run Container API ,DB
    ```bash
    docker-compose up --build
    ```

3. เปิด Swagger UI เพื่อทดสอบ API ได้ที่  

    [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

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
