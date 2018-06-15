

select * from tm_v_UserInfo



select * from tm_v_UserGroupFieldDisplayed
where FUserGroupNumber='002'


exec tm_p_GetFieldDisplayed '13011111111','002'

select * from t_Item_3036

select * from tm_v_SalesReturnHead


select * from tm_v_SalesReturnNotice
order by  3 desc



Exec tm_p_GetSalesReturnNotice @PhoneNumber= N'13011111111'

Exec tm_p_GetSalesOrderList '13044444444'

select * from tm_v_SeOrderList

