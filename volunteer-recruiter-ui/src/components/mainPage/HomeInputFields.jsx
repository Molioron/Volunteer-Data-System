import React from 'react';
import './HomePage.css';
const HomeInputFields = ({ volunteerAreas, jobTypes, initialDate, lastDate, dateFilterType, onChange }) => {
  return (
    <div className="home-input-fields">
      <div className="form-group">
        <label>Area:</label>
        <select name="volunteerAreas" value={volunteerAreas} onChange={(e) => onChange(e)}>
          <option value="">Select Area</option>
          <option value="North">North</option>
          <option value="South">South</option>
          <option value="Central">Central</option>
        </select>
      </div>

      <div className="form-group">
        <label>Job Title:</label>
        <select name="jobTypes" value={jobTypes} onChange={(e) => onChange(e)}>
          <option value="">Select Job Title</option>
          <option value="Agriculture">Agriculture</option>
          <option value="Cooking">Cooking</option>
          <option value="Transportation">Transportation</option>
          <option value="AnimalCare">Animal Care</option>
        </select>
      </div>

      <div className="form-group">
        <label>Initial Date:</label>
        <input
          type="date"
          name="initialDate"
          value={initialDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>End Date:</label>
        <input
          type="date"
          name="lastDate"
          value={lastDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>Date Type:</label>
        <select name="dateFilterType" value={dateFilterType} onChange={(e) => onChange(e)}>
          <option value="">Select Date Type</option>
          <option value="Contains">Contains</option>
          <option value="Intersects">Intersects</option>
        </select>
      </div>
    </div>
  );
};

export default HomeInputFields;
