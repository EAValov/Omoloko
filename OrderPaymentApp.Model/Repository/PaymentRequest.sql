use Master
go

create database OrderPaymentApp
on primary (
	name = N'OrderPaymentApp',
	filename = N'D:\Base\OrderPaymentApp.mdf',
	size = 3072kb,
	filegrowth = 1024kb
)
log on (
	name = N'OrderPaymentApp_log',
	filename = N'D:\Base\OrderPaymentApp_log.ldf',
	size = 1024kb,
	filegrowth = 10%
)
go

alter authorization on database::OrderPaymentApp to sa;
go

create login order_payment_service with password=N'K3pMJP', default_database=OrderPaymentApp, default_language=us_english, check_expiration=off, check_policy=off
go

use OrderPaymentApp
go

create schema OrderPayment;
go

create user order_payment_service for login order_payment_service with default_schema=OrderPaymentApp
go


create table OrderPayment.OrderStatuses (
	OrderStatusID smallint not null primary key identity(1,1),
	Text nvarchar(20) not null
);

insert into OrderPayment.OrderStatuses(Text)
	values	(N'New'),
			(N'Payment'),
			(N'Completed');
go

-- =============================================
-- Author: Валов Е.А.
-- Create date: 25.07.2020
-- Description: Получение Id статуса по тексту
-- =============================================
create function OrderPayment.GetOrderStatusByText(@Text nvarchar(20))
returns smallint
as
begin
	declare @OrderStatusID smallint

	select @OrderStatusID = OrderStatusID
	from OrderPayment.OrderStatuses
	where Text = @Text

	return @OrderStatusID
end

go		

create table OrderPayment.Orders (
	OrderID int not null primary key identity(1,1),	
	DT datetime not null default getdate(),
	Number nvarchar(20) not null unique,
	ClientEmail nvarchar(max) not null,
	OrderStatusID smallint not null
		foreign key references OrderPayment.OrderStatuses (OrderStatusID) 
		default OrderPayment.GetOrderStatusByText(N'New')
);

create table OrderPayment.PaymentRequestStatuses (
	PaymentRequeststatusID smallint not null primary key identity(1,1),
	Text nvarchar(20) not null
)

insert into OrderPayment.PaymentRequestStatuses(Text)
	values	(N'New'),
			(N'Rejected'),
			(N'Payed');
go

-- =============================================
-- Author: Валов Е.А.
-- Create date: 25.07.2020
-- Description: Получение Id статуса по тексту
-- =============================================
create function OrderPayment.GetPaymentRequestStatusByText(@Text nvarchar(20))
returns smallint
as
begin
	declare @PaymentRequeststatusID smallint

	select @PaymentRequeststatusID = PaymentRequeststatusID
	from OrderPayment.PaymentRequestStatuses
	where Text = @Text

	return @PaymentRequeststatusID
end;
go

create table OrderPayment.PaymentRequests(
	PaymentRequestID bigint not null primary key identity(1,1),
	OrderID int not null foreign key references OrderPayment.Orders (OrderID),
	DT datetime not null default getdate(),
	PaymentRequestStatusID smallint not null 
		foreign key references OrderPayment.PaymentRequestStatuses (PaymentRequestStatusID)
		default OrderPayment.GetPaymentRequestStatusByText(N'New')
);

go

-- =============================================
-- Author: Валов Е.А.
-- Create date: 25.07.2020
-- Description: Создание запроса на оплату.
-- =============================================
create procedure OrderPayment.CreatePaymentRequest(@OrderID int, @PaymentRequestID bigint output)
as
begin
begin transaction
	begin try

		if exists(select 1 from OrderPayment.PaymentRequests where OrderID = @OrderID)
		begin
			update pr
				set PaymentRequestStatusID = OrderPayment.GetPaymentRequestStatusByText(N'Rejected')
			from OrderPayment.PaymentRequests pr
			where pr.OrderID = @OrderID;			
		end

		update o
			set o.OrderStatusID = OrderPayment.GetOrderStatusByText(N'Payment')
		from OrderPayment.Orders o
		where o.OrderID = @OrderID;
		
		insert into PaymentRequests(OrderID, PaymentRequestStatusID)
			values( @OrderID, OrderPayment.GetPaymentRequestStatusByText(N'New'));

		set @PaymentRequestID = SCOPE_IDENTITY()

		if (@@TRANCOUNT > 0)
			commit transaction
	end try
	begin catch
		if (@@TRANCOUNT > 0)
			rollback transaction;

		declare @error_msg nvarchar(4000) = error_message();
		declare @error_severity int = error_severity();
		declare @error_state int = error_state();

		raiserror(@error_msg,  @error_severity, @error_state);
	end catch	
end;
go

-- =============================================
-- Author: Валов Е.А.
-- Create date: 25.07.2020
-- Description: Завершение оплаты.
-- =============================================
create procedure OrderPayment.CompletePaymentRequest(@PaymentRequestID int)
as
begin
begin transaction
	begin try
			update pr
				set PaymentRequestStatusID = OrderPayment.GetPaymentRequestStatusByText(N'Payed')
			from OrderPayment.PaymentRequests pr
			where pr.PaymentRequestID = @PaymentRequestID;
			
			update o
				set o.OrderStatusID = OrderPayment.GetOrderStatusByText(N'Completed')
			from OrderPayment.Orders o
				join OrderPayment.PaymentRequests pr on o.OrderID = pr.OrderID
			where pr.PaymentRequestID = @PaymentRequestID;

		if (@@TRANCOUNT > 0)
			commit transaction
	end try
	begin catch
		if (@@TRANCOUNT > 0)
			rollback transaction;

		declare @error_msg nvarchar(4000) = error_message();
		declare @error_severity int = error_severity();
		declare @error_state int = error_state();

		raiserror(@error_msg,  @error_severity, @error_state);
	end catch	
end;
go

-- =============================================
-- Author: Валов Е.А.
-- Create date: 25.07.2020
-- Description: Получение заявки по ID запроса на оплату.
-- =============================================
create function OrderPayment.GetOrderByPaymentRequestID (@paymentRequestID bigint)
returns table
as
return	
	select o.* from OrderPayment.Orders o
		join OrderPayment.PaymentRequests pr on pr.OrderID = o.OrderID
	where pr.PaymentRequestID = @paymentRequestID;

go

grant execute on schema::OrderPayment to [order_payment_service]
go

grant select on schema::OrderPayment to [order_payment_service]
go

insert into OrderPayment.Orders (Number, ClientEmail)
	values	(N'200723-111111', N'Valov@svel.ru'),
			(N'200723-111112', N'Valov@svel.ru'),
			(N'200723-111113', N'Valov@svel.ru')

/*скрипт с подсчетом кол-ва сгенерированных ссылок и кол-вом успешно оплаченных с разбивкой по дням.*/
select o.Number, cast(pr.DT as date) Date, count(distinct New) New, count(distinct Rejected) Rejected, count(distinct Payed) Payed
from OrderPayment.Orders o
	join OrderPayment.PaymentRequests pr on pr.OrderID = o.OrderID
	left join (	select pr_n.OrderID, cast(pr_n.DT as date) DT, pr_n.PaymentRequestID New
				from  OrderPayment.PaymentRequests pr_n
					join OrderPayment.PaymentRequestStatuses s_n on pr_n.PaymentRequestStatusID = s_n.PaymentRequeststatusID
				where s_n.Text = N'New') n on n.OrderID = o.OrderID and cast(pr.DT as date) = n.DT
	left join (	select pr_r.OrderID, cast(pr_r.DT as date) DT, pr_r.PaymentRequestID Rejected
				from  OrderPayment.PaymentRequests pr_r
					join OrderPayment.PaymentRequestStatuses s_r on pr_r.PaymentRequestStatusID = s_r.PaymentRequeststatusID
				where s_r.Text = N'Rejected') r on r.OrderID = o.OrderID and cast(pr.DT as date) = r.DT
	left join (	select pr_p.OrderID, cast(pr_p.DT as date) DT, pr_p.PaymentRequestID Payed
				from  OrderPayment.PaymentRequests pr_p
					join OrderPayment.PaymentRequestStatuses s_p on pr_p.PaymentRequestStatusID = s_p.PaymentRequeststatusID
				where s_p.Text = N'Payed') p on p.OrderID = o.OrderID and cast(pr.DT as date) = p.DT
group by o.Number, cast(pr.DT as date)
order by o.Number, cast(pr.DT as date)

