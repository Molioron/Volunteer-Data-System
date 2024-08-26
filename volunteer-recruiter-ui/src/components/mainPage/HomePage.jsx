import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import HomeInputFields from './HomeInputFields';
import './HomePage.css';

const HomePage = () => {
  const location = useLocation();
  const [formData, setFormData] = useState({
    volunteerAreas: [],
    jobTypes: [],
    initialDate: '',
    lastDate: '',
    dateFilterType: '',
  });
  const [posts, setPosts] = useState(location.state?.posts || []); 
  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  useEffect(() => {
    if (location.state?.posts) {
      setPosts(location.state.posts);
    }
  }, [location.state?.posts]);

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
      const operationStatus = JSON.parse(result.operationStatus);

      if (operationStatus.Code === 'Success') {
        setPosts(JSON.parse(result.posts)); // Set the posts state with the returned posts
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

  const handleMyPosts = async () => {
    try {
      const connectionKey = document.cookie.split('=')[1];
      const response = await fetch('http://localhost:9000/getuserposts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ connectionKey: connectionKey }),
      });

      const result = await response.json();
      const operationStatus = JSON.parse(result.operationStatus);

      if (operationStatus.Code === 'Success') {
        const userPosts = JSON.parse(result.posts); // Parse the user's posts
        navigate('/my-posts', { state: { posts: userPosts } }); // Navigate to MyPostsPage with posts data
      } else {
        alert('No posts found');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  const formatDate = (dateString) => {
    let [year, month, day] = dateString.split('-');
    day = day.split('T')[0];
    day = day.length === 1 ? `0${day}` : day;
    return `${day}.${month}.${year}`;
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
        <h2>Posts</h2>
        {posts.length > 0 ? (
          posts.map((post) => (
            <div key={post.Id} className="post-item">
              <h3>{post.Title}</h3>
              <p><strong>Description:</strong> {post.Description}</p>
              <p><strong>Volunteer Area:</strong> {post.Location}</p>
              <p><strong>Job:</strong> {post.Job}</p>
              <p><strong>Address:</strong> {post.Address}</p>
              <p><strong>Phone Number:</strong> {post.PhoneNumber}</p>
              <p><strong>Dates:</strong> {formatDate(post.InitialDate)} - {formatDate(post.LastDate)}</p> 
            </div>
          ))
        ) : (
          <p>No posts to display.</p>
        )}
      </div>
    </div>
  );
};

export default HomePage;
