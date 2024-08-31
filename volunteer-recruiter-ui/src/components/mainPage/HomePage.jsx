import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import HomeInputFields from './HomeInputFields';
import './HomePage.css';
import Button from '../common/Button';
import { handleLogout, PostItem } from '../../utils/GeneralUtils';
import useFormInput from '../../hooks/UseFormInput';
import { apiRequest, BASE_URL } from '../../utils/ApiUtils';
import { fetchUserPosts } from '../../utils/NavigationUtils';


const HomePage = () => {

  // Get location object to access state passed from previous route
  const location = useLocation();

  // Custom hook to manage form state; initialize with default values
  const { formData, handleInputChange} = useFormInput({
    volunteerAreas: [],
    jobTypes: [],
    initialDate: '',
    lastDate: '',
    dateFilterType: '',
  });


  // State to hold the posts; initialized with posts passed via location state or an empty array
  const [posts, setPosts] = useState(location.state?.posts || []); 

  const navigate = useNavigate();

  // Effect hook to update posts state if posts are passed via location state
  useEffect(() => {
    if (location.state?.posts) {
      setPosts(location.state.posts);
    }
  }, [location.state?.posts]);

  // Function to fetch filtered posts based on the form data
  const fetchFilteredPosts = async () => {
    try {
      const result = await apiRequest( BASE_URL + '/getfilteredposts', 'POST', formData);

      const posts = JSON.parse(result.posts);
      setPosts(posts);
    } catch (error) {
      alert('Error: ' + error.message);
    }  
  };

  return (
    <div className="home-page">
      <Button className="logout-button" onClick={() => handleLogout(navigate)}>Logout</Button>
      <div className="input-fields-container">
        <HomeInputFields
          area={ formData.volunteerAreas }
          jobTitle={ formData.jobTypes }
          initialDate={ formData.initialDate }
          lastDate={ formData.lastDate }
          dateType={ formData.dateFilterType }
          onChange={ handleInputChange }
        />
        <div className="action-buttons">
          <Button onClick={ fetchFilteredPosts }>Search</Button>
          <Button onClick={() => fetchUserPosts(navigate)}>My Posts</Button>
        </div>
      </div>
      <div className="content-container">
        {/* Here you will render the posts */}
        <h2>Posts</h2>
        {posts.length > 0 ? posts.map(post => <PostItem key={post.Id} post={post} />) : <p>No posts to display.</p>}
      </div>
    </div>
  );
};

export default HomePage;
