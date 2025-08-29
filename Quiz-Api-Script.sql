create database QuizApi;
use QuizApi;
CREATE TABLE Users (
    Id INT NOT NULL AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);
SELECT * FROM Users;
SELECT * FROM Users WHERE Name = "James bond" AND Id != 5;
DELETE  FROM Users;


CREATE TABLE QUIZ (
    Quiz_Id INT NOT NULL AUTO_INCREMENT,
    Question VARCHAR(500) NOT NULL,
    Correct_Answer_Id INT,  
    PRIMARY KEY (Quiz_Id)
);
select * from QUIZ;
select * from Possible_Answer;
CREATE TABLE Possible_Answer (
    P_Ans_Id INT NOT NULL AUTO_INCREMENT,
    Quiz_Id INT NOT NULL,   
    Letter CHAR(1),        
    Sentence VARCHAR(500) NOT NULL,
    
    PRIMARY KEY (P_Ans_Id),
    FOREIGN KEY (Quiz_Id) REFERENCES QUIZ(Quiz_Id)
        ON DELETE CASCADE
);

ALTER TABLE QUIZ
ADD CONSTRAINT FK_CorrectAnswer
FOREIGN KEY (Correct_Answer_Id) REFERENCES Possible_Answer(P_Ans_Id)
    ON DELETE SET NULL;


INSERT INTO QUIZ (Question) VALUES
('What is the capital of France?'),
('What is the capital of Japan?');

INSERT INTO Possible_Answer (Quiz_Id, Letter, Sentence) VALUES
-- For Quiz 1
(1, 'A', 'Paris'),
(1, 'B', 'London'),
(1, 'C', 'Berlin'),
(1, 'D', 'Madrid'),

-- For Quiz 2
(2, 'A', 'Seoul'),
(2, 'B', 'Beijing'),
(2, 'C', 'Tokyo'),
(2, 'D', 'Bangkok');

UPDATE QUIZ SET Correct_Answer_Id = 17 WHERE Quiz_Id = 1; -- Paris
UPDATE QUIZ SET Correct_Answer_Id = 23 WHERE Quiz_Id = 2; -- Tokyo

SELECT q.Quiz_Id, q.Question, pa.Letter, pa.Sentence,
       IF(q.Correct_Answer_Id = pa.P_Ans_Id, 'Correct', '') AS Status
FROM QUIZ q
JOIN Possible_Answer pa ON q.Quiz_Id = pa.Quiz_Id
ORDER BY q.Quiz_Id, pa.Letter;


