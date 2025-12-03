-
SELECT "Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine"
	FROM public."ToDoItem"
    ORDER BY "CreatedAt";


SELECT "Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine"
	FROM public."ToDoItem"
	WHERE "State" = 0
    ORDER BY "CreatedAt";

DO $$
DECLARE
	"RandomUserId" uuid;
	"RandomListId" uuid;
	"RandomStateChangedAt" date;
	"RandomDeadLine" date;
BEGIN
  	SELECT "UserId" INTO "RandomUserId" FROM "ToDoUser" ORDER BY random() LIMIT 1;
  	SELECT "Id" INTO "RandomListId" FROM "ToDoList" ORDER BY random() LIMIT 1;
	SELECT ('2025-12-01 10:00:00'::timestamp + (('2025-12-12 12:00:00'::timestamp - '2025-12-01 10:00:00'::timestamp)* random())) INTO "RandomStateChangedAt";
	SELECT ('2025-11-01 10:00:00'::timestamp + (('2025-11-12 12:00:00'::timestamp - '2025-11-01 10:00:00'::timestamp)* random())) INTO "RandomDeadLine";

	INSERT INTO public."ToDoItem" ("Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine")
    VALUES (gen_random_uuid()::uuid,
			"RandomUserId",
			"RandomListId",
			'Task_' || random(150, 200)::varchar(100),
			default,
			0,
			"RandomStateChangedAt",
			"RandomDeadLine");
END $$


UPDATE public."ToDoItem"
	SET "State" = 1
	WHERE "Id" IN (SELECT "Id"
		 	   	   FROM public."ToDoItem"
			   	   ORDER BY random()
			   	   LIMIT 1);


DELETE FROM public."ToDoItem"
	WHERE "Id" IN (SELECT "Id"
		 	   	   FROM public."ToDoItem"
			   	   ORDER BY random()
			   	   LIMIT 1);



SELECT EXISTS(
		SELECT 1
		FROM public."ToDoItem"
		WHERE "Name" = 'Task_12'
		AND "UserId" IN (SELECT "UserId"
		 	   	   	 FROM public."ToDoUser"
			   	   	 ORDER BY random()
			   	   	 LIMIT 1));


SELECT COUNT(*)
	FROM public."ToDoItem"
	WHERE "State" = 0;


SELECT "Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine"
	FROM public."ToDoItem"
	WHERE "Name" LIKE 'Task_1%'
    ORDER BY "CreatedAt";
