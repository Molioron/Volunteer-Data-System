import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Auth.css';
import AuthInputField from './AuthInputField';
import Button from '../common/Button';
import { navigateToHomePage } from '../../utils/NavigationUtils';
import useFormInput from '../../hooks/UseFormInput';
import { apiRequest, BASE_URL } from '../../utils/ApiUtils';

/**
 * `Auth` component manages the authentication state and handles switching between 
 * login and signup forms.
 * 
 * Uses:
 * - `useNavigate` (hook): Redirects user post-authentication.
 * - `useFormInput` (hook): Manages form data state.
 * - `apiRequest` (function): Makes API calls to the login or signup endpoints.
 * - `navigateToHomePage` (function): Redirects to the homepage upon successful login/signup.
 */

const Auth = () => {

  const navigate = useNavigate();

  const [isSignup, setIsSignup] = useState(false);
  
  const { formData, handleInputChange, setFormData } = useFormInput({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: ''
  });

    /**
   * Toggles between login and signup forms, and resets form data.
   * @param {boolean} flag - True for signup, false for login.
   */

  const handleFormSwitch = (flag) => {
    setIsSignup(flag);
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      phone: ''
    });
  };

    /**
   * Submits login/signup form data to the relevant API endpoint.
   * @param {Event} e - Form submission event.
   */

  const handleSubmit = async (e) => {
    e.preventDefault();
  
    const url = isSignup ? BASE_URL + '/signup' : BASE_URL + '/login';

    try {
      const result = await apiRequest(url, 'POST', formData);
      handleResponse(result);
    } catch (error) {
      alert('Error: ' + error.message);
    }
  };

    /**
   * Handles response from the server, redirecting upon success or showing an alert upon failure.
   * @param {Object} response - Server response object containing operation status and connection key.
   */

  const handleResponse = async (response) => {
    try {
      const operationStatus = JSON.parse(response.operationStatus);
      if (operationStatus.Code === 'Success') {
        document.cookie = `connectionKey=${response.connectionKey}; path=/;`;
        await navigateToHomePage(navigate);
      } else {
        alert('Failed: ' + operationStatus.Message);
      }
    } catch (error) {
      alert('Error: Invalid response format');
    }

  };

  return (
    <div className="auth-page">
      <div className="auth-toggle-buttons">
        <Button onClick={() => handleFormSwitch(false)}>Log In</Button>
        <Button onClick={() => handleFormSwitch(true)}>Sign Up</Button>
      </div>
      <AuthInputField
        isSignup={isSignup}
        formData={formData}
        handleInputChange={handleInputChange}
        handleSubmit={handleSubmit}
      />
    </div>
  );
};

export default Auth;
