CREATE DATABASE IF NOT EXISTS smeretail CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE smeretail;

-- Seed currencies for Zimbabwe
INSERT INTO currencies (currency_code, currency_name, symbol, decimal_places, is_base_currency, is_active)
VALUES 
  ('USD', 'US Dollar', '$', 2, TRUE, TRUE),
  ('ZiG', 'Zimbabwe Gold', 'ZiG', 2, FALSE, TRUE)
ON DUPLICATE KEY UPDATE currency_name = VALUES(currency_name);

-- Seed VAT categories
INSERT INTO vat_categories (category_name, vat_rate, is_active)
VALUES 
  ('Standard Rate', 15.00, TRUE),
  ('Zero Rated', 0.00, TRUE),
  ('Exempt', 0.00, TRUE)
ON DUPLICATE KEY UPDATE vat_rate = VALUES(vat_rate);

-- Seed units of measure
INSERT INTO units_of_measure (uom_code, uom_name)
VALUES 
  ('EA', 'Each'),
  ('KG', 'Kilogram'),
  ('L', 'Litre'),
  ('BOX', 'Box'),
  ('PACK', 'Pack')
ON DUPLICATE KEY UPDATE uom_name = VALUES(uom_name);

-- Seed warehouses
INSERT INTO warehouses (warehouse_code, warehouse_name, location, is_active)
VALUES 
  ('MAIN', 'Main Warehouse', 'Harare', TRUE),
  ('SHOP', 'Retail Shop Floor', 'Harare', TRUE)
ON DUPLICATE KEY UPDATE warehouse_name = VALUES(warehouse_name);
