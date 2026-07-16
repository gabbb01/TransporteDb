-- =============================================
-- SCRIPT: Agregar columna CostoLempiras a la tabla Rutas
-- Fórmula: MIN(MAX(DistanciaKm * 3, 10), 60)
-- Mínimo: 10 LPS | Máximo: 60 LPS
-- =============================================

USE TransporteDb;
GO

-- 1. Agregar la columna con valor por defecto de 10 LPS
ALTER TABLE Rutas
ADD CostoLempiras INT NOT NULL DEFAULT 10;
GO

-- 2. Actualizar los costos según la distancia: MIN(MAX(Km * 3, 10), 60)
UPDATE Rutas
SET CostoLempiras = 
    CASE 
        WHEN DistanciaKm * 3 < 10 THEN 10
        WHEN DistanciaKm * 3 > 60 THEN 60
        ELSE ROUND(DistanciaKm * 3, 0)
    END;
GO

-- 3. Verificar los resultados
SELECT 
    r.RutaId,
    eo.Nombre AS Origen,
    ed.Nombre AS Destino,
    r.DistanciaKm,
    r.CostoLempiras,
    r.TiempoMinutos
FROM Rutas r
INNER JOIN Estaciones eo ON r.OrigenId = eo.EstacionId
INNER JOIN Estaciones ed ON r.DestinoId = ed.EstacionId
ORDER BY r.CostoLempiras DESC;
GO
