import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import HomeInputFields from './HomeInputFields';
import './HomePage.css';

const HomePage = () => {
  const [formData, setFormData] = useState({
    Location: '',
    Job: '',
    initialDate: '',
    endDate: '',
    DateFilterType: '',
  });

  // const [cookieData, setCookieData] = useState({
  //   connectionKey: ''
  // });


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

      const result = await response.json();
      console.log(result);
      const operationStatus = JSON.parse(result.operationStatus);
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
      console.log({connectionKey: connectionKey});

      const response = await fetch('http://localhost:9000/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify( {connectionKey: connectionKey} ),
      });

      const result = await response.json();
      console.log(result);

      const operationStatus = JSON.parse(result.operationStatus);

      console.log(operationStatus.Code);
      if (operationStatus.Code === "Success") {
        alert(operationStatus.Message);
        document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        navigate('/');
      } else {
        console.error('Error1:', result.operationStatus.Message);
        alert('Error1: ' + result.operationStatus.Message);
      }
    } catch (error) {
      console.error('Error2:', error);
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
          Location={formData.Location}
          Job={formData.Job}
          initialDate={formData.initialDate}
          endDate={formData.endDate}
          DateFilterType={formData.DateFilterType}
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
