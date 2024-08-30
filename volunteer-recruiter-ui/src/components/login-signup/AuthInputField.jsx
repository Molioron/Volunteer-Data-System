import React from 'react';
import Button from '../common/Button';
import InputField from '../common/InputField';

const AuthInputField = ({ isSignup, formData, handleInputChange, handleSubmit }) => {
  return (
    // When the form is submitted, call the handleSubmit function
    <form onSubmit={handleSubmit}> 
      {isSignup ? ( // If isSignup is true, display Signup form fields
        <>
          <h2>Signup</h2>
          <InputField label="First Name:" type="text" name="firstName" value={formData.firstName} onChange={handleInputChange} required autoComplete="given-name" />
          <InputField label="Last Name:" type="text" name="lastName" value={formData.lastName} onChange={handleInputChange} required autoComplete="family-name" />
          <InputField label="Email:" type="email" name="email" value={formData.email} onChange={handleInputChange} required autoComplete="email" />
          <InputField label="Password:" type="password" name="password" value={formData.password} onChange={handleInputChange} required autoComplete="new-password" />
          <InputField label="Phone:" type="tel" name="phone" value={formData.phone} onChange={handleInputChange} required autoComplete="tel" />
        </>
      ) : ( // If isSignup is false, display Login form fields
        <>
          <h2>Login</h2>
          <InputField label="Email:" type="email" name="email" value={formData.email} onChange={handleInputChange} required autoComplete="email" />
          <InputField label="Password:" type="password" name="password" value={formData.password} onChange={handleInputChange} required autoComplete="current-password" />
        </>
      )}
      <div className="auth-action-button">
        <Button type="submit">Submit</Button>
      </div>
    </form>
  );
};

export default AuthInputField;
