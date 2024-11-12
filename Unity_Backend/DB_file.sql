-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 12, 2024 at 08:02 PM
-- Server version: 10.4.24-MariaDB
-- PHP Version: 8.1.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `unity_backend`
--

-- --------------------------------------------------------

--
-- Table structure for table `pants`
--

CREATE TABLE `pants` (
  `id` int(11) NOT NULL,
  `length` int(11) DEFAULT NULL,
  `waist` int(11) DEFAULT NULL,
  `imagepath` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `pants`
--

INSERT INTO `pants` (`id`, `length`, `waist`, `imagepath`) VALUES
(1, 30, 14, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Pants_images/pant1.jpg'),
(2, 28, 18, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Pants_images/pant2.jpg'),
(3, 33, 18, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Pants_images/pant3.jpg'),
(4, 32, 19, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Pants_images/pant4.jpg'),
(5, 27, 16, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Pants_images/pant5.jpg'),
(6, 31, 16, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Pants_images/pant6.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `shirts`
--

CREATE TABLE `shirts` (
  `id` int(11) NOT NULL,
  `imagePath` varchar(255) DEFAULT NULL,
  `height` int(11) DEFAULT NULL,
  `width` int(11) DEFAULT NULL,
  `neck` int(11) DEFAULT NULL,
  `shoulder` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `shirts`
--

INSERT INTO `shirts` (`id`, `imagePath`, `height`, `width`, `neck`, `shoulder`) VALUES
(1, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Shirts_images/shirt.jpg', 24, 16, 15, 19),
(2, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Shirts_images/shirt2.jpg', 23, 17, 16, 18),
(3, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Shirts_images/shirt3.jpg', 24, 14, 14, 16),
(4, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Shirts_images/shirt4.jpg', 23, 17, 13, 22),
(5, 'C:/Users/Maari/Unity Projects/sample_project2/Assets/Shirts_images/shirt5.jpg', 26, 16, 23, 15);

-- --------------------------------------------------------

--
-- Table structure for table `usermeasurements`
--

CREATE TABLE `usermeasurements` (
  `userMeasurementID` int(11) NOT NULL,
  `userID` int(11) DEFAULT NULL,
  `distance_shoulder` float(4,2) DEFAULT NULL,
  `distance_waist` float(4,2) DEFAULT NULL,
  `distance_arms` float(4,2) DEFAULT NULL,
  `distance_arms2` float(4,2) DEFAULT NULL,
  `distance_legs` float(4,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `usermeasurements`
--

INSERT INTO `usermeasurements` (`userMeasurementID`, `userID`, `distance_shoulder`, `distance_waist`, `distance_arms`, `distance_arms2`, `distance_legs`) VALUES
(3, 2, 16.96, 17.53, 22.54, 22.15, 28.48),
(11, 3, 17.97, 16.91, 22.01, 21.88, 22.13),
(23, 1, 17.10, 16.22, 23.84, 21.75, 28.59),
(25, 6, 16.96, 17.53, 22.54, 22.15, 28.48);

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Username` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`UserID`, `Name`, `Username`, `Password`) VALUES
(1, 'maarib', 'maarib123', 'abc123'),
(2, 'wasay', 'wasay123', 'wasay123'),
(3, 'shoaib', 'shoaib4321', '4321'),
(5, 'sampleuser', 'sampleuser', '12345'),
(6, 'sampleuser2', 'sampleuser2', '12345');

-- --------------------------------------------------------

--
-- Table structure for table `users_images`
--

CREATE TABLE `users_images` (
  `ImageID` int(11) NOT NULL,
  `UserID` int(11) DEFAULT NULL,
  `ImagePath` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `users_images`
--

INSERT INTO `users_images` (`ImageID`, `UserID`, `ImagePath`) VALUES
(3, 2, 'C:UsersMaariUnity Projectssample_project2Assetsuser_imageswasay123_image.png'),
(4, 1, 'C:UsersMaariUnity Projectssample_project2Assetsuser_imagesmaarib123_image.png'),
(5, 3, 'C:UsersMaariUnity Projectssample_project2Assetsuser_imagesshoaib4321_image.png'),
(6, 6, 'C:\\Users\\Maari\\Unity Projects\\sample_project2\\Assets\\user_images\\sampleuser2_image.png');

-- --------------------------------------------------------

--
-- Table structure for table `users_measurments`
--

CREATE TABLE `users_measurments` (
  `image_id` int(11) NOT NULL,
  `distance_shoulder` float(4,2) DEFAULT NULL,
  `distance_waist` float(4,2) DEFAULT NULL,
  `distance_arms` float(4,2) DEFAULT NULL,
  `distance_arms2` float(4,2) DEFAULT NULL,
  `distance_legs` float(4,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `users_measurments`
--

INSERT INTO `users_measurments` (`image_id`, `distance_shoulder`, `distance_waist`, `distance_arms`, `distance_arms2`, `distance_legs`) VALUES
(1, 20.63, 26.09, 22.27, 20.86, 24.02),
(2, 17.10, 16.22, 23.84, 21.75, 28.59),
(3, 17.10, 16.22, 23.84, 21.75, 28.59),
(4, 17.10, 16.22, 23.84, 21.75, 28.59),
(14, 25.02, 35.92, 20.09, 18.91, 28.05),
(15, 19.32, 18.72, 27.57, 25.57, 33.09),
(16, 17.10, 16.22, 23.84, 21.75, 28.59),
(17, 17.10, 16.22, 23.84, 21.75, 28.59);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `pants`
--
ALTER TABLE `pants`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `shirts`
--
ALTER TABLE `shirts`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `usermeasurements`
--
ALTER TABLE `usermeasurements`
  ADD PRIMARY KEY (`userMeasurementID`),
  ADD KEY `userID` (`userID`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `Username` (`Username`);

--
-- Indexes for table `users_images`
--
ALTER TABLE `users_images`
  ADD PRIMARY KEY (`ImageID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `users_measurments`
--
ALTER TABLE `users_measurments`
  ADD PRIMARY KEY (`image_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `pants`
--
ALTER TABLE `pants`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `shirts`
--
ALTER TABLE `shirts`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `usermeasurements`
--
ALTER TABLE `usermeasurements`
  MODIFY `userMeasurementID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `users_images`
--
ALTER TABLE `users_images`
  MODIFY `ImageID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `users_measurments`
--
ALTER TABLE `users_measurments`
  MODIFY `image_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `usermeasurements`
--
ALTER TABLE `usermeasurements`
  ADD CONSTRAINT `usermeasurements_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `users` (`UserID`);

--
-- Constraints for table `users_images`
--
ALTER TABLE `users_images`
  ADD CONSTRAINT `users_images_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
