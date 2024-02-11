/* Admins table */
CREATE TABLE `inventory_system`.`admins` (
    `id` INT NOT NULL AUTO_INCREMENT ,
    `name` VARCHAR(255) NOT NULL ,
    PRIMARY KEY (`id`), UNIQUE `ADMIN_NAME_UNIQUE` (`name`)
) ENGINE = InnoDB;

/* Rooms table */
CREATE TABLE `inventory_system`.`rooms` (
    `id` INT NOT NULL AUTO_INCREMENT ,
    `name` VARCHAR(255) NOT NULL ,
    `admin_id` INT NOT NULL ,
    PRIMARY KEY (`id`), UNIQUE `ROOM_NAME_UNIQUE` (`name`)
) ENGINE = InnoDB;
ALTER TABLE `rooms` ADD CONSTRAINT `ADMIN_ID_FK` FOREIGN KEY (`admin_id`) REFERENCES `admins`(`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

/* Items table */
CREATE TABLE `inventory_system`.`items` (
    `id` INT NOT NULL AUTO_INCREMENT ,
    `name` VARCHAR(255) NOT NULL ,
    `price` INT NOT NULL ,
    `start` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ,
    `end` TIMESTAMP NULL DEFAULT NULL ,
    `room_id` INT NOT NULL ,
    PRIMARY KEY (`id`), UNIQUE `ITEM_NAME_UNIQUE` (`name`)
) ENGINE = InnoDB;
ALTER TABLE `items` ADD CONSTRAINT `ROOM_ID_FK` FOREIGN KEY (`room_id`) REFERENCES `rooms`(`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;