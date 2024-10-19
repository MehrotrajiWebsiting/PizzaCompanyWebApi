# PizzaCompanyWebApi
A simple Web API which implements .NET 8.0, EF Core and SQL Server to create Web API
There are 5 main tables and 1 join table used - 
  1. Users
  2. Roles
  3. Orders
  4. Products
  5. RoleChangeRequests
  6. UserRoles (join table for Users and Roles many to many relationship)

# Brief Overview
  1. Created a Web API using .NET 8.0 for managing Pizza orders by different users with different roles.
  2. Implemented functionalities for creating, updating, and managing customer and their orders using ADO.NET, LINQ, and EF Core.
  3. The project also implements user registration and login to perform operations (on role basis). It also implements role based authorization as well as role update requests by users (approved/rejected/read only by admin role).
  4. The project supports CRUD operations (GET, POST, PUT, DELETE) for orders and products and authentication/authorization using JWT roles.
  5. Integrated best practices and Repository design pattern for scalable and secure API development.
