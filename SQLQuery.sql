CREATE TABLE WeatherDegree(
	[Id] int IDENTITY PRIMARY KEY,
	[Degree] decimal(5,2),
	[WeatherDate] datetime
);

GO

CREATE TYPE weatherData AS TABLE   
    ( WeatherDate datetime
    , Degree decimal(5,2) );  
GO  

CREATE PROCEDURE SetWeather  @weahterData weatherData READONLY
AS
BEGIN
	INSERT INTO WeatherDegree(Degree, WeatherDate)
	SELECT degree, weatherDate
	FROM @weahterData as B
	WHERE CAST(weatherDate AS DATE) BETWEEN CAST(GETDATE() AS DATE) AND CAST(DATEADD(DD,7,GETDATE()) AS DATE)
	AND NOT EXISTS(SELECT 1 FROM WeatherDegree T WHERE T.WeatherDate = B.WeatherDate)
END
GO

CREATE PROCEDURE SetOrUpdateWeather @weahterData weatherData READONLY
AS
BEGIN

	MERGE WeatherDegree AS Target
	USING (SELECT Degree, WeatherDate
	FROM @weahterData) AS Source
	ON Source.weatherDate = Target.WeatherDate

	WHEN NOT MATCHED BY Target THEN
    INSERT (Degree, WeatherDate) 
    VALUES (degree, weatherDate)

	 WHEN MATCHED THEN UPDATE SET
     Target.Degree	= Source.degree,
     Target.WeatherDate = Source.weatherDate;

END
GO




CREATE PROCEDURE GetUpcomingWeater
AS
BEGIN
	DROP TABLE IF EXISTS ##weatherTemp;

	SELECT CAST(WeatherDate AS DATE) AS weatherDate, AVG(Degree) degree
	INTO ##weatherTemp
	FROM WeatherDegree
	WHERE CAST(WeatherDate AS DATE) > CAST(GETDATE() AS DATE)
	GROUP BY CAST(WeatherDate AS DATE);

	SELECT WeatherDate, Degree, UpcomingDate
	FROM ##weatherTemp AS t1
	CROSS APPLY (
		SELECT MIN(weatherDate) upcomingDate
		FROM ##weatherTemp AS t2
		WHERE t2.WeatherDate > t1.weatherDate AND t2.Degree > T1.degree
	) AS t3;
END

GO


CREATE PROCEDURE GetWeather @day datetime
AS
BEGIN
	SELECT WeatherDate, Degree
	FROM WeatherDegree
	WHERE CAST(WeatherDate AS DATE) = CAST(@day AS DATE);
END


GO

CREATE PROC DeleteOldData
AS
BEGIN
DELETE FROM WeatherDegree
WHERE CAST(weatherDate AS DATE) < CAST(DATEADD(DD,-30,GETDATE()) AS DATE)
END