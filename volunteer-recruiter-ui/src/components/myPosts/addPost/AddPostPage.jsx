import { useNavigate, useLocation } from 'react-router-dom';
import React, { useEffect } from 'react';
import AddPostInputField from './AddPostInputField';
import './AddPostPage.css';
import Button from '../../common/Button';
import { handleLogout } from '../../../utils/GeneralUtils';
import useFormInput from '../../../hooks/UseFormInput';
import { apiRequest, BASE_URL } from '../../../utils/ApiUtils';
import { fetchUserPosts } from '../../../utils/NavigationUtils';

const AddPostPage = () => {

  const navigate = useNavigate();
  const location = useLocation();
  const post = location.state?.post; // Access the post data if passed for editing

  // Custom hook to manage form data state
  const { formData, handleInputChange, setFormData } = useFormInput({
    connectionKey: '',
    title: '',
    description: '',
    address: '',
    volunteerArea: '',
    jobType: '',
    initialDate: '',
    lastDate: '',
  });

  useEffect(() => {
    if (post) {
      // If post is passed, prefill the form with post data
      setFormData({
        title: post.Title,
        description: post.Description,
        address: post.Address,
        volunteerArea: post.Location,
        jobType: post.Job,
        initialDate: post.InitialDate,
        lastDate: post.LastDate,
      });
    }
  }, [post, setFormData]); // Dependency array ensures this effect runs only when post or setFormData changes

  // Function to handle form submission for creating or updating a post
  const handleSubmit = async (e) => {
    e.preventDefault();
  
    // Determine the API endpoint based on whether we are creating or editing a post
    const urlSuffix = post ? '/editpost' : '/createpost';
    const url = BASE_URL + urlSuffix;
    const connectionKey = document.cookie.split('=')[1]; // Get the connection key from cookies
    const updateformData = { ...formData, connectionKey: connectionKey, id: post?.Id }; // Prepare form data for submission
  
    try {
      const result = await apiRequest(url, 'POST', updateformData);
      handleResponse(result, urlSuffix);
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  // Function to handle the response after API call
  const handleResponse = async (response, operation) => {
    try {
      const operationStatus = JSON.parse(response.operationStatus); // Parse the operation status from the response

      if (operationStatus.Code === 'Success') {
        if (operation === '/createpost') {
          alert('Post created successfully');
        } else {
          alert('Post updated successfully');
        }

        // Fetch updated user posts after successful operation
        const result = await apiRequest(BASE_URL + '/getuserposts', 'POST', { connectionKey: document.cookie.split('=')[1] });
        const userPosts = JSON.parse(result.posts);
        navigate('/my-posts', { state: { posts: userPosts } }); // Navigate to 'My Posts' page with updated posts
      } else {
        alert('Error: ' + operationStatus.Message);
      }
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

  return (
    <div className="add-post-page">
      <Button className="logout-button" onClick={() => handleLogout(navigate)}>Logout</Button>
      <div className='add-post'>
        <AddPostInputField
          title={formData.title}
          description={formData.description}
          address={formData.address}
          volunteerArea={formData.volunteerArea}
          jobType={formData.jobType}
          initialDate={formData.initialDate}
          lastDate={formData.lastDate}
          onChange={handleInputChange}
        />
      </div>
      <div className="add-post-action-button">
        <Button onClick={() => fetchUserPosts(navigate)}>My Posts</Button>
        <Button onClick={handleSubmit}>{post ? 'Update Post' : 'Create Post'}</Button>
      </div>
    </div>
  );
};

export default AddPostPage;
