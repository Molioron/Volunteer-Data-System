import React from 'react';

const HomeInputFields = ({ area, jobTitle, initialDate, endDate, dateType, onChange }) => {
  return (
    <div className="input-fields">
      <div className="form-group">
        <label>Area:</label>
        <select name="area" value={area} onChange={(e) => onChange(e)}>
          <option value="">Select Area</option>
          <option value="North">North</option>
          <option value="South">South</option>
          <option value="Central">Central</option>
        </select>
      </div>

      <div className="form-group">
        <label>Job Title:</label>
        <select name="jobTitle" value={jobTitle} onChange={(e) => onChange(e)}>
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
          name="endDate"
          value={endDate}
          onChange={(e) => onChange(e)}
        />
      </div>

      <div className="form-group">
        <label>Date Type:</label>
        <select name="dateType" value={dateType} onChange={(e) => onChange(e)}>
          <option value="">Select Date Type</option>
          <option value="Contains">Contains</option>
          <option value="Intersects">Intersects</option>
        </select>
      </div>
    </div>
  );
};

export default HomeInputFields;
