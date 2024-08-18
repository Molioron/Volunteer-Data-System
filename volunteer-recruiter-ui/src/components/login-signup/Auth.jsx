import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Auth.css';
import InputField from './InputField';

const Auth = () => {

  const navigate = useNavigate();

  const [isSignup, setIsSignup] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: ''
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

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

  const handleSubmit = async (e) => {
    e.preventDefault();

    const dataToSend = { ...formData };

    console.log('Form Data:', dataToSend); // Log form data to the console
    console.log((dataToSend.email).type);

    const url = isSignup ? 'http://localhost:9000/signup' : 'http://localhost:9000/login';
    console.log('URL:', url); // Log URL to the console
    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
      });
      const result = await response.json();
      handleResponse(result, 'login-signup');
    } catch (error) {
      console.error('Error:', error);
      alert('Error222: ' + error.message);
    }
  };
  const handleResponse = (response) => {
      console.log(response);
      try {
        const operationStatus = JSON.parse(response.operationStatus);
        if (operationStatus.Code === 'Success') {
          alert(operationStatus.Message);
          document.cookie = `connectionKey=${response.connectionKey}; path=/;`;
          navigate('/main');
        } else if(operationStatus.Code === 'AlreadyExistsError' || operationStatus.Code === 'CredentialsError') {
          alert('Error: ' + operationStatus.Message);
        } else {
          alert('Failed: ' + operationStatus.Message);
        }
      } catch (error) {
        console.error('Error parsing response:', error);
        alert('Error: Invalid response format');
      }
  };

  return (
    <div>
      <div>
        <button onClick={() => handleFormSwitch(false)}>Log In</button>
        <button onClick={() => handleFormSwitch(true)}>Sign Up</button>
      </div>
      {isSignup ? (
        <div>
          <h2>Signup</h2>
          <form onSubmit={handleSubmit}>
          <div className="form-group">
          <InputField 
              label="First Name:" 
              type="text" 
              name="firstName" 
              value={formData.firstName} 
              onChange={handleInputChange} 
              required 
            />
          </div>
            
            <div className="form-group">
            <InputField 
              label="Last Name:" 
              type="text" 
              name="lastName" 
              value={formData.lastName} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <div className="form-group">
            <InputField 
              label="Email:" 
              type="email" 
              name="email" 
              value={formData.email} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <div className="form-group">
            <InputField 
              label="Password:" 
              type="password" 
              name="password" 
              value={formData.password} 
              onChange={handleInputChange} 
              required 
            />
            </div>

            <div className="form-group">
            <InputField 
              label="Phone:" 
              type="phone" 
              name="phone" 
              value={formData.phone} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <button type="submit">Submit</button>
          </form>
        </div>
      ) : (
        <div>
          <h2>Login</h2>
          <form onSubmit={handleSubmit}>
          <div className="form-group">
          <InputField 
              label="Email:" 
              type="email" 
              name="email" 
              value={formData.email} 
              onChange={handleInputChange} 
              required 
            />
          </div>
            
          <div className="form-group">
          <InputField 
              label="Password:" 
              type="password" 
              name="password" 
              value={formData.password} 
              onChange={handleInputChange} 
              required 
            />
          </div>
            
            <button type="submit">Submit</button>
          </form>
        </div>
      )}
    </div>
  );
};

export default Auth;
