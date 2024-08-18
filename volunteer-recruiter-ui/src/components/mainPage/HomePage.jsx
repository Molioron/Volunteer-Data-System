import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import HomeInputFields from './HomeInputFields';
import './HomePage.css';

const HomePage = () => {
  const [formData, setFormData] = useState({
    area: '',
    jobTitle: '',
    initialDate: '',
    endDate: '',
    dateType: '',
  });

  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSearch = () => {
    const params = new URLSearchParams(formData).toString();

    // Send GET request with query parameters
    fetch(`http://localhost:9000/api/search?${params}`)
      .then(response => response.json())
      .then(data => {
        console.log('Received data:', data);
        // Handle the received data
      })
      .catch(error => {
        console.error('Error:', error);
      });
  };

  const handleLogout = async () => {
    try {
      const response = await fetch('http://localhost:9000/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: document.cookie }),
      });
      console.log(JSON.stringify({ connectionKey: document.cookie}));

      console.log(JSON.stringify({ connectionKey: document.cookie.split('=')[1] })); // Log form data to the console
      const result = await response.json();
      console.log(result.operationStatus.Code);
      if (result.operationStatus.Code === 'Success') {
        alert(result.operationStatus.Message);
        document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        navigate('/');
      } else {
        alert('Error1: ' + result.operationStatus.Message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error2: ' + error.message);
    }
  };

  const handleMyPosts = () => {
    navigate('/my-posts'); // This will be the route to the new page you'll create later
  };

  return (
    <div className="home-page">
      <div className="input-fields-container">
      <button className="logout-button" onClick={handleLogout}>Logout</button>
        <HomeInputFields
          area={formData.area}
          jobTitle={formData.jobTitle}
          initialDate={formData.initialDate}
          endDate={formData.endDate}
          dateType={formData.dateType}
          onChange={handleInputChange}
        />
      </div>
      <div className="content-container">
        {/* This is the big rectangle container */}
      </div>
      <div className="action-buttons">
        <button onClick={handleSearch}>Search</button>
        <button onClick={handleMyPosts}>My Posts</button>
      </div>
    </div>
  );
};

export default HomePage;
