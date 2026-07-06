-- =============================================
-- PROYECTO 3: Sistema de Rutas y Transporte Urbano
-- Base de Datos - Red de Transporte de Comayagua
-- Rutas: Comayagua, La Paz, Ajuterique, El Rosario
-- =============================================

CREATE DATABASE TransporteDb
GO

USE TransporteDb;
GO

-- =============================================
-- TABLA: Estaciones (Nodos del Grafo)
-- =============================================
CREATE TABLE Estaciones (
    EstacionId INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(20) UNIQUE NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Ubicacion VARCHAR(150),
    Activa BIT DEFAULT 1
);
GO

-- =============================================
-- TABLA: Rutas (Aristas del Grafo)
-- =============================================
CREATE TABLE Rutas (
    RutaId INT IDENTITY(1,1) PRIMARY KEY,
    OrigenId INT NOT NULL,
    DestinoId INT NOT NULL,
    DistanciaKm DECIMAL(5,2) NOT NULL,
    TiempoMinutos INT NOT NULL,

    CONSTRAINT FK_Rutas_Origen FOREIGN KEY (OrigenId) REFERENCES Estaciones(EstacionId),
    CONSTRAINT FK_Rutas_Destino FOREIGN KEY (DestinoId) REFERENCES Estaciones(EstacionId),
    CONSTRAINT CK_Rutas_NoAutoRef CHECK (OrigenId <> DestinoId)
);
GO

CREATE INDEX IX_Rutas_OrigenId ON Rutas(OrigenId);
CREATE INDEX IX_Rutas_DestinoId ON Rutas(DestinoId);
GO

-- =============================================
-- ESTACIONES DE COMAYAGUA (Ciudad)
-- =============================================
INSERT INTO Estaciones (Codigo, Nombre, Ubicacion, Activa) VALUES
('COM-01', 'Terminal de Buses',       'Terminal de Transporte - Comayagua',                          1),
('COM-02', 'Parque Central',          'Plaza León Alvarado - Catedral Inmaculada Concepción',        1),
('COM-03', 'UNAH CURC',              'Salida a Tegucigalpa, Col. San Miguel - contiguo a Ferromax', 1),
('COM-04', 'Hospital Santa Teresa',   'Hospital Regional Santa Teresa - Comayagua',                  1),
('COM-05', 'Mall Premier',            'Barrio Cabañas - antiguo Estadio Hispano',                    1),
('COM-06', 'Mercado Municipal',       'Mercado San Francisco - Centro de Comayagua',                 1),
('COM-07', 'Estadio Carlos Miranda',  'Estadio Municipal Carlos Miranda - Comayagua',                1),
('COM-08', 'Barrio Arriba',           'Barrio Arriba - Iglesia La Merced',                           1);
GO

-- =============================================
-- ESTACIONES DE LA PAZ (~23 km desde Comayagua)
-- =============================================
INSERT INTO Estaciones (Codigo, Nombre, Ubicacion, Activa) VALUES
('PAZ-01', 'Entrada La Paz',          'Desvío entrada a La Paz desde carretera principal',           1),
('PAZ-02', 'Parque Central La Paz',   'Parque Central - Cabecera Departamental de La Paz',           1),
('PAZ-03', 'Terminal La Paz',         'Terminal de Buses - La Paz',                                  1);
GO

-- =============================================
-- ESTACIONES DE AJUTERIQUE (~13 km al sur de Comayagua)
-- =============================================
INSERT INTO Estaciones (Codigo, Nombre, Ubicacion, Activa) VALUES
('AJU-01', 'Entrada Ajuterique',      'Desvío Ajuterique - Carretera al Sur',                       1),
('AJU-02', 'Parque Ajuterique',       'Parque Central - Ajuterique',                                1);
GO

-- =============================================
-- ESTACIONES DE EL ROSARIO (~20 km al norte de Comayagua)
-- =============================================
INSERT INTO Estaciones (Codigo, Nombre, Ubicacion, Activa) VALUES
('ROS-01', 'Entrada El Rosario',      'Desvío El Rosario - Carretera al Norte, faldas Cerro Grande', 1),
('ROS-02', 'Centro El Rosario',       'Parque Central - El Rosario (San Antonio de Opoteca)',        1);
GO

-- =============================================
-- RUTAS INTERNAS DE COMAYAGUA
-- =============================================

-- Terminal de Buses (COM-01)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (1, 2, 1.20, 5);    -- Terminal → Parque Central
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (1, 6, 0.80, 4);    -- Terminal → Mercado Municipal
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (1, 7, 1.50, 7);    -- Terminal → Estadio

-- Parque Central (COM-02)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (2, 4, 1.80, 8);    -- Parque Central → Hospital
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (2, 6, 0.50, 3);    -- Parque Central → Mercado
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (2, 8, 1.00, 5);    -- Parque Central → Barrio Arriba
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (2, 5, 1.50, 7);    -- Parque Central → Mall Premier

-- UNAH CURC (COM-03)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (3, 5, 2.80, 10);   -- CURC → Mall Premier
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (3, 7, 2.00, 9);    -- CURC → Estadio
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (3, 4, 2.20, 10);   -- CURC → Hospital

-- Hospital Santa Teresa (COM-04)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (4, 5, 2.50, 12);   -- Hospital → Mall Premier
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (4, 8, 1.30, 6);    -- Hospital → Barrio Arriba

-- Mall Premier (COM-05)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (5, 1, 2.00, 8);    -- Mall → Terminal (regreso)

-- Mercado Municipal (COM-06)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (6, 1, 0.80, 4);    -- Mercado → Terminal
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (6, 8, 1.20, 6);    -- Mercado → Barrio Arriba

-- Estadio Carlos Miranda (COM-07)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (7, 3, 2.00, 9);    -- Estadio → CURC
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (7, 2, 1.80, 8);    -- Estadio → Parque Central

-- Barrio Arriba (COM-08)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (8, 2, 1.00, 5);    -- Barrio Arriba → Parque Central
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (8, 3, 3.00, 12);   -- Barrio Arriba → CURC

-- =============================================
-- RUTAS COMAYAGUA → LA PAZ (~23 km)
-- =============================================
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (1, 9, 18.00, 30);  -- Terminal → Entrada La Paz (por carretera principal)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (5, 9, 16.50, 25);  -- Mall Premier → Entrada La Paz (salida norte)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (9, 10, 3.50, 8);   -- Entrada La Paz → Parque Central La Paz
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (9, 11, 4.00, 10);  -- Entrada La Paz → Terminal La Paz
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (10, 11, 1.00, 4);  -- Parque La Paz → Terminal La Paz
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (11, 9, 4.00, 10);  -- Terminal La Paz → Entrada (regreso)

-- =============================================
-- RUTAS COMAYAGUA → AJUTERIQUE (~13 km al sur)
-- =============================================
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (1, 12, 10.00, 20); -- Terminal → Entrada Ajuterique
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (7, 12, 9.50, 18);  -- Estadio → Entrada Ajuterique (ruta alterna por el sur)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (12, 13, 3.00, 8);  -- Entrada Ajuterique → Parque Ajuterique
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (13, 12, 3.00, 8);  -- Parque Ajuterique → Entrada (regreso)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (12, 1, 10.00, 20); -- Entrada Ajuterique → Terminal (regreso)

-- =============================================
-- RUTAS COMAYAGUA → EL ROSARIO (~20 km al norte)
-- =============================================
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (1, 14, 18.00, 35);  -- Terminal → Entrada El Rosario
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (14, 15, 4.00, 10);  -- Entrada El Rosario → Centro El Rosario
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (15, 14, 4.00, 10);  -- Centro El Rosario → Entrada (regreso)
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (14, 1, 18.00, 35);  -- Entrada El Rosario → Terminal (regreso)

-- =============================================
-- RUTAS INTER-MUNICIPALES
-- =============================================
INSERT INTO Rutas (OrigenId, DestinoId, DistanciaKm, TiempoMinutos) VALUES (13, 14, 20.00, 45); -- Ajuterique → El Rosario (sur a norte)

GO

-- =============================================
-- CONSULTAS DE VERIFICACIÓN
-- =============================================

-- Ver todas las estaciones
SELECT * FROM Estaciones ORDER BY Codigo;

-- Ver rutas con nombres de estaciones
SELECT 
    r.RutaId,
    eo.Codigo AS CodigoOrigen,
    eo.Nombre AS Origen,
    ed.Codigo AS CodigoDestino,
    ed.Nombre AS Destino,
    r.DistanciaKm,
    r.TiempoMinutos
FROM Rutas r
INNER JOIN Estaciones eo ON r.OrigenId = eo.EstacionId
INNER JOIN Estaciones ed ON r.DestinoId = ed.EstacionId
ORDER BY eo.Codigo, r.DistanciaKm;

-- Resumen de conectividad por estación
SELECT 
    e.Codigo,
    e.Nombre,
    COUNT(r.RutaId) AS TotalConexiones
FROM Estaciones e
LEFT JOIN Rutas r ON e.EstacionId = r.OrigenId
GROUP BY e.Codigo, e.Nombre
ORDER BY TotalConexiones DESC;

GO
