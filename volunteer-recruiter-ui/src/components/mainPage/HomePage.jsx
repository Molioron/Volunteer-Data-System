import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import HomeInputFields from './HomeInputFields';
import './HomePage.css';

const HomePage = () => {
  const [formData, setFormData] = useState({
    volunteerAreas: '',
    jobTypes: '',
    initialDate: '',
    lastDate: '',
    dateFilterType: '',
  });

  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSearch = async () => {
    try {
      const response = await fetch('http://localhost:9000/getfilteredposts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });
      
      console.log(JSON.stringify(formData));
      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);
      console.log(result);
      if (operationStatus.Code === 'Success') {
        alert(operationStatus.Message);
      } else if (operationStatus.Code === 'ServerError') {
        alert(operationStatus.Message);
      } else {
        alert('No posts found');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }  
  };

  const handleLogout = async () => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await fetch('http://localhost:9000/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: connectionKey }),
      });

      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);

      if (operationStatus.Code === "Success") {
        alert(operationStatus.Message);
        document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        navigate('/');
      } else {
        alert('Error: ' + result.operationStatus.Message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const handleMyPosts = () => {
    navigate('/my-posts');
  };

  return (
    <div className="home-page">
      <button className="logout-button" onClick={handleLogout}>Logout</button>
      <div className="input-fields-container">
        <HomeInputFields
          area={formData.volunteerAreas}
          jobTitle={formData.jobTypes}
          initialDate={formData.initialDate}
          endDate={formData.lastDate}
          dateType={formData.dateFilterType}
          onChange={handleInputChange}
        />
        <div className="action-buttons">
          <button onClick={handleSearch}>Search</button>
          <button onClick={handleMyPosts}>My Posts</button>
        </div>
      </div>
      <div className="content-container">
        {/* Here you will render the posts */}
        <h2>Posts</h2>
        {/* Example placeholder content */}
        <div className="post-item">
          <p>This is where the posts will be displayed.</p>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
