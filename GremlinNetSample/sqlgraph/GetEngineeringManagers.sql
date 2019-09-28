SELECT sqlgraph.employee.name FROM sqlgraph.employee
INNER JOIN sqlgraph.employee_group ON sqlgraph.employee_group.employeeID = sqlgraph.Employee.ID
INNER JOIN sqlgraph.Group ON sqlgraph.Employee_Group.GroupID = sqlgraph.Group.ID
INNER JOIN 
	(
	SELECT DISTINCT EmployeeID FROM sqlgraph.employee_reportEmployee 
	) as T
	ON T.EmployeeID = sqlgraph.Employee.ID
WHERE sqlgraph.group.Name = 'Engineering';