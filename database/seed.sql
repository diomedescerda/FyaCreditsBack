-- Sample credits from the technical test annex.
-- Commercial representative and registration dates are illustrative.
INSERT INTO "credits" ("Id", "ClientName", "ClientId", "Amount", "InterestRate", "TermMonths", "CommercialName", "RegisteredAtUtc")
SELECT gen_random_uuid(), data.client_name, data.client_id, data.amount, 2.00, data.term_months, 'Ana Comercial', NOW() - (data.offset_days || ' days')::interval
FROM (VALUES
    ('Pepito Perez',       '1001',  7800000, 10, 20),
    ('Maria Perez',        '1002', 12500000,  5, 19),
    ('Antonio Rodriguez',  '1003', 10312673,  5, 18),
    ('Giselle López',      '1004',  8628510, 12, 17),
    ('Martha Perez',       '1005',  5889085, 24, 16),
    ('Isaac llanos',       '1006', 14793565, 48, 15),
    ('Teresa Gutierrez',   '1007',  8072348, 50, 14),
    ('Isabel Llanos',      '1008',  5143860, 60, 13),
    ('Paola Tao',          '1009', 12881963, 24, 12),
    ('Wendy Moscoso',      '1010', 13484682, 40, 11)
) AS data(client_name, client_id, amount, term_months, offset_days);
